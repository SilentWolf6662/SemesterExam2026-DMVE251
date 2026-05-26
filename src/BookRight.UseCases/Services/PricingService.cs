using BookRight.Domain.Entities;
using BookRight.Domain.Enums;
using BookRight.Domain.Exceptions;
using BookRight.UseCases.Discount;
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

    // Beregner den endelige pris for en booking, inklusive eventuelle rabatter.
    public async Task<decimal> Calculate(Appointment appointment)
    {
        Lock _lock = new();

        // Hvis bookingen ikke har en pris defineret, kastes en Exception
        if (!appointment.HasPrice) throw new DomainException("Appointment skal have en pris for at kunne beregnes.");

        // Beregn basisprisen baseret på BookRight-reglerne ved at ligge overtidsgebyret oveni,
        // hvis bookingen evt. er om aftenen eller i weekenden
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

        // Hent en eventuel kampagne der gælder for bookingens tidspunkt, for at kunne inkludere kampagnerabat i beregningen
        var campaign = await _campaignRepository.GetCampaignForAppointmentTimeAsync(appointment.TimeInterval.End);

        // Læg vædierne i seperate variabler, hvis der ikke er en kampagne bruges defaultværdier
        var campaignDiscountAmount = campaign?.DiscountRate ?? 0; // Default rabat er 0, hvis ingen kampagne
        var campaignName = campaign?.Name ?? string.Empty; // Default navn er tom string, hvis ingen kampagne

        // Lav et DiscountInput-objekt som indeholder alle de nødvendige informationer for at kunne beregne rabatterne i de forskellige strategier
        var discountInput = new DiscountInput(
            currentPrice,
            appointment.TimeInterval.End,
            appointment.TimeInterval.Start,
            patientBirthday,
            patientBooking12MonthTotalSum,
            birthdayDiscountUsedCount,
            campaignDiscountAmount,
            campaignName
        );

        DiscountResult? bestDiscount = null;

        // Hent alle rabatter fra de forskellige strategier og find den bedste (højeste) rabat på en thread-safe måde
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

    // Beregner den endelige pris for en KOMBINERET booking som én enhed.
    // Rabatten beregnes på den samlede pris og fordeles proportionalt mellem de to aftaler,
    // så fødselsdagsrabatten (og andre rabatter) kun bruges én gang for hele besøget.
    public async Task<CombinedPriceResult> CalculateCombined(Appointment first, Appointment second)
    {
        if (!first.HasPrice || !second.HasPrice)
            throw new DomainException("Begge aftaler skal have en pris for at kunne beregnes.");

        decimal firstOvertimePrice = OvertimeCharge.Calculate(first);
        decimal secondOvertimePrice = OvertimeCharge.Calculate(second);
        decimal totalPrice = firstOvertimePrice + secondOvertimePrice;

        var patient = await _patientRepository.GetByIdAsync(first.PatientId)
            ?? throw new DomainException("Patienten for bookingen kunne ikke findes.");

        var patientBirthday = DateOnly.FromDateTime(patient.Birthday);
        var patientBooking12MonthTotalSum = await _appointmentRepository.GetSumOf12MonthsByPatientIdAsync(first.PatientId, first.TimeInterval.Start);
        var birthdayDiscountUsedCount = await _appointmentRepository.GetBirthdayDiscountUsedCountByPatientIdAsync(first.PatientId, first.TimeInterval.Start);

        var discountInput = new DiscountInput(
            totalPrice,
            second.TimeInterval.End,
            first.TimeInterval.Start,
            patientBirthday,
            patientBooking12MonthTotalSum,
            birthdayDiscountUsedCount);

        Lock combinedLock = new();
        DiscountResult? bestDiscount = null;

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
        decimal firstShare = totalPrice > 0 ? firstOvertimePrice / totalPrice : 0.5m;
        decimal firstDiscount = Math.Round(totalDiscount * firstShare, 2);
        decimal secondDiscount = totalDiscount - firstDiscount;

        return new CombinedPriceResult(
            Math.Round(firstOvertimePrice - firstDiscount, 2),
            Math.Round(secondOvertimePrice - secondDiscount, 2),
            bestDiscount?.DiscountType ?? DiscountType.None);
    }

    // Beregner en prisestimering med fuldt breakdown — bruges til at vise pris FØR booking oprettes.
    // Kræver at behandlingstype, varighed, patient og starttidspunkt er valgt.
    public async Task<PriceBreakdown> CalculatePreview(Guid treatmentTypeId, int durationMinutes, Guid patientId, DateTime from)
    {
        var treatmentType = await _treatmentTypeRepository.GetByIdAsync(treatmentTypeId)
            ?? throw new DomainException("Behandlingstypen kunne ikke findes.");

        decimal basePrice = treatmentType.GetBasePrice(durationMinutes);

        // Overtidsberegning — samme logik som OvertimeCharge.Calculate men uden at kræve et Appointment-objekt
        bool isEvening = TimeOnly.FromDateTime(from) >= new TimeOnly(17, 0);
        bool isWeekend = from.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
        decimal priceAfterOvertime = (isEvening || isWeekend) ? Math.Round(basePrice * 1.15m, 2) : basePrice;
        decimal overtimeSurcharge = priceAfterOvertime - basePrice;

        var patient = await _patientRepository.GetByIdAsync(patientId)
            ?? throw new DomainException("Patienten kunne ikke findes.");

        var patientBirthday = DateOnly.FromDateTime(patient.Birthday);
        var to = from.AddMinutes(durationMinutes);
        var patientBooking12MonthTotalSum = await _appointmentRepository.GetSumOf12MonthsByPatientIdAsync(patientId, from);
        var birthdayDiscountUsedCount = await _appointmentRepository.GetBirthdayDiscountUsedCountByPatientIdAsync(patientId, from);

        var discountInput = new DiscountInput(
            priceAfterOvertime,
            to,
            from,
            patientBirthday,
            patientBooking12MonthTotalSum,
            birthdayDiscountUsedCount);

        Lock previewLock = new();
        DiscountResult? bestDiscount = null;

        Parallel.ForEach(_discountStrategies, strategy =>
        {
            var discount = strategy.Calculate(discountInput).GetAwaiter().GetResult();
            lock (previewLock)
            {
                if (discount.IsApplicable && (bestDiscount == null || discount.DiscountAmount > bestDiscount.DiscountAmount))
                    bestDiscount = discount;
            }
        });

        decimal discountAmount = Math.Round(bestDiscount?.DiscountAmount ?? 0, 2);
        decimal finalPrice = Math.Round(priceAfterOvertime - discountAmount, 2);

        return new PriceBreakdown(
            basePrice,
            overtimeSurcharge,
            priceAfterOvertime,
            bestDiscount,
            finalPrice);
    }

    // Prisestimering for kombineret booking — én rabat beregnes på det samlede beløb.
    // Returnerer to PriceBreakdown-objekter (én per behandling) med proportional rabat.
    public async Task<(PriceBreakdown First, PriceBreakdown Second)> CalculateCombinedPreview(
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

        var patient = await _patientRepository.GetByIdAsync(patientId)
            ?? throw new DomainException("Patienten kunne ikke findes.");

        var patientBirthday = DateOnly.FromDateTime(patient.Birthday);
        var to2 = from2.AddMinutes(duration2);
        var patientBooking12MonthTotalSum = await _appointmentRepository.GetSumOf12MonthsByPatientIdAsync(patientId, from);
        var birthdayDiscountUsedCount = await _appointmentRepository.GetBirthdayDiscountUsedCountByPatientIdAsync(patientId, from);

        var discountInput = new DiscountInput(
            totalPrice,
            to2,
            from,
            patientBirthday,
            patientBooking12MonthTotalSum,
            birthdayDiscountUsedCount);

        Lock previewLock = new();
        DiscountResult? bestDiscount = null;

        Parallel.ForEach(_discountStrategies, strategy =>
        {
            var discount = strategy.Calculate(discountInput).GetAwaiter().GetResult();
            lock (previewLock)
            {
                if (discount.IsApplicable && (bestDiscount == null || discount.DiscountAmount > bestDiscount.DiscountAmount))
                    bestDiscount = discount;
            }
        });

        decimal totalDiscount = Math.Round(bestDiscount?.DiscountAmount ?? 0, 2);

        // Fordel rabatten proportionalt
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
