using BookRight.Domain.Entities;
using BookRight.Domain.Enums;
using BookRight.Domain.Exceptions;
using BookRight.UseCases.Discount;
using BookRight.UseCases.Repositories;

namespace BookRight.UseCases.Services;

// PricingService er en domæne-service til prisberegning.
// Den er statisk fordi den ikke har nogen tilstand.
// Den transformer blot en indgangsværdi til en udgangsværdi baseret på faste forretningsregler.
public class PricingService
{
    private readonly IEnumerable<IDiscountStrategy> _discountStrategies;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IPatientRepository _patientRepository;

    public PricingService(IEnumerable<IDiscountStrategy> discountStrategies, IAppointmentRepository appointmentRepository, IPatientRepository patientRepository)
    {
        _discountStrategies = discountStrategies;
        _appointmentRepository = appointmentRepository;
        _patientRepository = patientRepository;
    }

    // Beregner den endelige pris for en booking.
    
    public async Task<decimal> Calculate(Appointment appointment)
    {
        Lock _lock = new();

        // Hvis bookingen ikke har en pris defineret, kastes en Exception
        if (!appointment.HasPrice) throw new DomainException("Appointment skal have en pris for at kunne beregnes.");

        // Beregn basisprisen baseret på BookRight-reglerne, ved at ligge overtidsgebyret oveni,
        // hvis bookingen er om aftenen eller i weekenden
        decimal currentPrice = OvertimeCharge.Calculate(appointment);

        // Hent patienten for at finde deres fødselsdag
        var patient = await _patientRepository.GetByIdAsync(appointment.PatientId);

        // Hvis patienten ikke kan hentes, kastes en Exception
        if (patient == null) throw new DomainException("Patienten for bookingen kunne ikke findes.");

        // Konverter patientens fødselsdag til DateOnly for at kunne sammenligne med bookingens dato
        var patientBirthday = DateOnly.FromDateTime(patient.Birthday);

        // Find summen af alle bookinger for patienten i de sidste 12 måneder, for at kunne beregne rabatter baseret på det
        var patientBooking12MonthTotalSum = await _appointmentRepository.GetSumOf12MonthsByPatientIdAsync(appointment.PatientId, appointment.TimeInterval.Start);

        // Find ud af om patienten har brugt sin fødselsdagsrabat 
        var birthdayDiscountUsedCount = await _appointmentRepository.GetBirthdayDiscountUsedCountByPatientIdAsync(appointment.PatientId, appointment.TimeInterval.Start);

        var discountInput = new DiscountInput(
            currentPrice,
            appointment.TimeInterval.End,
            appointment.TimeInterval.Start,
            patientBirthday,
            patientBooking12MonthTotalSum,
            birthdayDiscountUsedCount
            );

        DiscountResult? bestDiscount = null;

        // Hent alle rabatter fra de forskellige strategier og find den bedste (højeste) rabat
        Parallel.ForEach(_discountStrategies, strategy =>
        {
            var discount = strategy.Calculate(discountInput).GetAwaiter().GetResult();

            lock (_lock)
            {
                // Hvis rabbaten er anvendelig og enten ikke har en rabat endnu eller har en højere rabat end den nuværende rabat
                if (discount.IsApplicable == true && (bestDiscount == null || discount.DiscountAmount > bestDiscount.DiscountAmount))
                {
                    bestDiscount = discount; // Ersat den nye rabat med den gamle
                }
            }
        });

        // Beregn den endelige pris ved at trække den højeste rabat fra basisprisen
        return currentPrice - (bestDiscount?.DiscountAmount ?? 0);
    }
}
