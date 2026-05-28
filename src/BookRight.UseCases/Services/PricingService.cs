using BookRight.Domain.Exceptions;
using BookRight.Domain.Discount;
using BookRight.UseCases.Interfaces;
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
    private readonly ITreatmentTypeRepository _treatmentTypeRepository;
    private readonly ICampaignRepository _campaignRepository;

    public PricingService(
        IEnumerable<IDiscountStrategy> discountStrategies, 
        IAppointmentRepository appointmentRepository, 
        IPatientRepository patientRepository, 
        ITreatmentTypeRepository treatmentTypeRepository, 
        ICampaignRepository campaignRepository)
    {
        _discountStrategies = discountStrategies;
        _appointmentRepository = appointmentRepository;
        _patientRepository = patientRepository;
        _treatmentTypeRepository = treatmentTypeRepository;
        _campaignRepository = campaignRepository;
    }

    // Beregner det fulde prisregnskab for én booking.
    // Bruges både til preview (inden booking oprettes) og ved selve oprettelsen.
    public async Task<PriceBreakdown> Calculate(Guid treatmentTypeId, int durationMinutes, Guid patientId, DateTime from)
    {
        var treatmentType = await _treatmentTypeRepository.GetByIdAsync(treatmentTypeId)
            ?? throw new DomainException("Behandlingstypen kunne ikke findes.");

        // Beregn basisprisen baseret på BookRight-reglerne ved at ligge overtidsgebyret oveni,
        // hvis bookingen evt. er om aftenen eller i weekenden
        decimal basePrice = treatmentType.GetBasePrice(durationMinutes);

        // Overtidsberegning — samme logik som OvertimeCharge.Calculate men uden at kræve et Appointment-objekt
        // TODO: FIND UD AF AT BRUGE OvertimeCharge KLASSEN I STEDET
        bool isEvening = TimeOnly.FromDateTime(from) >= new TimeOnly(17, 0);
        bool isWeekend = from.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
        decimal priceAfterOvertime = (isEvening || isWeekend) ? Math.Round(basePrice * 1.15m, 2) : basePrice;
        decimal overtimeSurcharge = priceAfterOvertime - basePrice;

        // Hent patienten for at finde deres fødselsdag
        var patient = await _patientRepository.GetByIdAsync(patientId)
            ?? throw new DomainException("Patienten kunne ikke findes.");

        // Konverter patientens fødselsdag til DateOnly for at kunne sammenligne med bookingens dato
        var to = from.AddMinutes(durationMinutes);
        var patientBirthday = DateOnly.FromDateTime(patient.Birthday);

        // Find summen af alle bookinger for patienten i de sidste 12 måneder, for at kunne beregne rabatter baseret på det
        var patientBooking12MonthTotalSum = await _appointmentRepository.GetSumOf12MonthsByPatientIdAsync(patientId, from);

        // Find ud af om patienten har brugt sin fødselsdagsrabat
        var birthdayDiscountUsedCount = await _appointmentRepository.GetBirthdayDiscountUsedCountByPatientIdAsync(patientId, from);

        // Hent en eventuel kampagne der gælder for bookingens tidspunkt, for at kunne inkludere kampagnerabat i beregningen
        var activeCampaign = await _campaignRepository.GetCampaignForAppointmentTimeAsync(to);

        // Lav et DiscountInput-objekt som indeholder alle de nødvendige informationer for at kunne beregne rabatterne i de forskellige strategier
        var discountInput = new DiscountInput(
            priceAfterOvertime,
            to,
            from,
            patientBirthday,
            patientBooking12MonthTotalSum,
            birthdayDiscountUsedCount,
            activeCampaign?.DiscountRate ?? 0, // Default rabat er 0, hvis ingen kampagne
            activeCampaign?.Name ?? string.Empty); // Default navn er tom string, hvis ingen kampagne

        Lock strategyLock = new();
        DiscountResult? bestDiscount = null;

        // Hent alle rabatter fra de forskellige strategier og find den bedste (højeste) rabat på en thread-safe måde
        Parallel.ForEach(_discountStrategies, strategy =>
        {
            var discount = strategy.Calculate(discountInput).GetAwaiter().GetResult();

            // Lås for at sikre tråd-sikker adgang til bestDiscount, og undgå race conditions og lost updates, da Parallel.ForEach kører på flere tråde
            lock (strategyLock) 
            {
                // Hvis rabbatten er anvendelig og enten ikke har en rabat endnu eller har en højere rabat end den nuværende rabat
                if (discount.IsApplicable && (bestDiscount == null || discount.DiscountAmount > bestDiscount.DiscountAmount))
                {
                    bestDiscount = discount; // Erstat den nuværende rabat med den bedre
                }
            }
        });

        // Beregn den endelige pris ved at trække den højeste rabat fra basisprisenS
        decimal discountAmount = Math.Round(bestDiscount?.DiscountAmount ?? 0, 2);

        return new PriceBreakdown(
            basePrice,
            overtimeSurcharge,
            priceAfterOvertime,
            bestDiscount,
            Math.Round(priceAfterOvertime - discountAmount, 2));
    }

    // Beregner den endelige pris for en KOMBINERET booking som én enhed.
    // Bruges både til preview (inden booking oprettes) og ved selve oprettelsen.
    // Rabatten beregnes på den samlede pris og fordeles proportionalt mellem de to aftaler,
    // så fødselsdagsrabatten (og andre rabatter) kun bruges én gang for hele besøget.
    public async Task<(PriceBreakdown First, PriceBreakdown Second)> CalculateCombined(
        Guid treatment1Id, int duration1,
        Guid treatment2Id, int duration2,
        Guid patientId, DateTime from)
    {
        var type1 = await _treatmentTypeRepository.GetByIdAsync(treatment1Id)
            ?? throw new DomainException("Første behandlingstype kunne ikke findes.");
        var type2 = await _treatmentTypeRepository.GetByIdAsync(treatment2Id)
            ?? throw new DomainException("Anden behandlingstype kunne ikke findes.");

        decimal base1 = type1.GetBasePrice(duration1);
        decimal base2 = type2.GetBasePrice(duration2);

        // Overtidsberegning for 1. behandling
        bool eve1 = TimeOnly.FromDateTime(from) >= new TimeOnly(17, 0);
        bool wkd1 = from.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
        decimal price1 = (eve1 || wkd1) ? Math.Round(base1 * 1.15m, 2) : base1;

        // Overtidsberegning for 2. behandling (starter præcis når 1. slutter)
        var from2 = from.AddMinutes(duration1);
        bool eve2 = TimeOnly.FromDateTime(from2) >= new TimeOnly(17, 0);
        bool wkd2 = from2.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
        decimal price2 = (eve2 || wkd2) ? Math.Round(base2 * 1.15m, 2) : base2;

        decimal totalPrice = price1 + price2;

        // Hent patienten for at finde deres fødselsdag
        var patient = await _patientRepository.GetByIdAsync(patientId)
            ?? throw new DomainException("Patienten kunne ikke findes.");

        var patientBirthday = DateOnly.FromDateTime(patient.Birthday);
        var to2 = from2.AddMinutes(duration2);

        // Find summen af alle bookinger for patienten i de sidste 12 måneder
        var patientBooking12MonthTotalSum = await _appointmentRepository.GetSumOf12MonthsByPatientIdAsync(patientId, from);

        // Find ud af om patienten har brugt sin fødselsdagsrabat
        var birthdayDiscountUsedCount = await _appointmentRepository.GetBirthdayDiscountUsedCountByPatientIdAsync(patientId, from);

        // Hent en eventuel kampagne der gælder for bookingens tidspunkt
        var activeCampaign = await _campaignRepository.GetCampaignForAppointmentTimeAsync(to2);

        // Lav et DiscountInput-objekt med alle nødvendige informationer for at beregne rabatter
        var discountInput = new DiscountInput(
            totalPrice,
            to2,
            from,
            patientBirthday,
            patientBooking12MonthTotalSum,
            birthdayDiscountUsedCount,
            activeCampaign?.DiscountRate ?? 0, // Default rabat er 0, hvis ingen kampagne
            activeCampaign?.Name ?? string.Empty // Default navn er tom string, hvis ingen kampagne
        );

        Lock combinedLock = new();
        DiscountResult? bestDiscount = null;

        // Hent alle rabatter fra de forskellige strategier og find den bedste (højeste) rabat på en thread-safe måde
        Parallel.ForEach(_discountStrategies, strategy =>
        {
            var discount = strategy.Calculate(discountInput).GetAwaiter().GetResult();
            lock (combinedLock)
            {
                if (discount.IsApplicable && (bestDiscount == null || discount.DiscountAmount > bestDiscount.DiscountAmount))
                    bestDiscount = discount;
            }
        });

        decimal totalDiscount = Math.Round(bestDiscount?.DiscountAmount ?? 0, 2);

        // Fordel rabatten proportionalt — summen af de to dele er altid lig totalDiscount
        decimal share1 = totalPrice > 0 ? price1 / totalPrice : 0.5m;
        decimal discount1 = Math.Round(totalDiscount * share1, 2);
        decimal discount2 = totalDiscount - discount1;

        // Opret nye DiscountResult med de proportionale beløb så BestDiscount altid er konsistent
        DiscountResult? result1 = bestDiscount is not null ? bestDiscount with { DiscountAmount = discount1 } : null;
        DiscountResult? result2 = bestDiscount is not null ? bestDiscount with { DiscountAmount = discount2 } : null;

        var breakdown1 = new PriceBreakdown(base1, price1 - base1, price1, result1, Math.Round(price1 - discount1, 2));
        var breakdown2 = new PriceBreakdown(base2, price2 - base2, price2, result2, Math.Round(price2 - discount2, 2));

        return (breakdown1, breakdown2);
    }
}
