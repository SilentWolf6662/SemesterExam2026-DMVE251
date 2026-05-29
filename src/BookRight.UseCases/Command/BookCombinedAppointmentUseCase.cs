using BookRight.Domain.Entities;
using BookRight.Domain.Enums;
using BookRight.Domain.Exceptions;
using BookRight.Domain.ValueObjects;
using BookRight.Facade.Command;
using BookRight.Facade.Interfaces.UseCase;
using BookRight.UseCases.Repositories;
using BookRight.UseCases.Services;

namespace BookRight.UseCases.Command;

public class BookCombinedAppointmentUseCase : IBookCombinedAppointment
{
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly IPractitionerRepository _practitionerRepo;
    private readonly IPatientRepository _patientRepo;
    private readonly ITreatmentTypeRepository _treatmentTypeRepo;
    private readonly IClinicRepository _clinicRepo;
    private readonly PricingService _pricingService;

    public BookCombinedAppointmentUseCase(
        IAppointmentRepository appointmentRepo,
        IPractitionerRepository practitionerRepo,
        IPatientRepository patientRepo,
        ITreatmentTypeRepository treatmentTypeRepo,
        IClinicRepository clinicRepo,
        PricingService pricingService)
    {
        _appointmentRepo = appointmentRepo;
        _practitionerRepo = practitionerRepo;
        _patientRepo = patientRepo;
        _treatmentTypeRepo = treatmentTypeRepo;
        _clinicRepo = clinicRepo;
        _pricingService = pricingService;
    }

    public async Task Execute(BookCombinedAppointmentRequest request)
    {
        _ = await _patientRepo.GetByIdAsync(request.PatientId)
            ?? throw new NotFoundException("Patient ikke fundet");

        _ = await _practitionerRepo.GetByIdAsync(request.FirstPractitionerId)
            ?? throw new NotFoundException("Første behandler ikke fundet");

        _ = await _practitionerRepo.GetByIdAsync(request.SecondPractitionerId)
            ?? throw new NotFoundException("Anden behandler ikke fundet");

        var clinic = await _clinicRepo.GetByIdAsync(request.ClinicId)
            ?? throw new NotFoundException("Klinik ikke fundet");

        var firstType = await _treatmentTypeRepo.GetByIdAsync(request.FirstTreatmentTypeId)
            ?? throw new NotFoundException("Første behandlingstype ikke fundet");

        var secondType = await _treatmentTypeRepo.GetByIdAsync(request.SecondTreatmentTypeId)
            ?? throw new NotFoundException("Anden behandlingstype ikke fundet");

        if (request.FirstTreatmentTypeId == request.SecondTreatmentTypeId)
            throw new DomainException("En kombineret booking skal indeholde to forskellige behandlingstyper");

        // De to tidsblokke er sammenhængende — anden starter præcis når første slutter
        var firstStart = request.From;
        var firstEnd = firstStart.AddMinutes(request.FirstDurationMinutes);
        var secondEnd = firstEnd.AddMinutes(request.SecondDurationMinutes);

        var firstInterval = new TimeInterval(firstStart, firstEnd);
        var secondInterval = new TimeInterval(firstEnd, secondEnd);

        decimal firstBasePrice = firstType.GetBasePrice(request.FirstDurationMinutes);
        decimal secondBasePrice = secondType.GetBasePrice(request.SecondDurationMinutes);

        // Rumsbegrænsning: begge tidsblokke tjekkes separat — de er ikke samtidige
        var clinicBookinger = (await _appointmentRepo.GetAllByClinicIdAsync(request.ClinicId)).ToList();
        var overlappende1 = clinicBookinger.Count(a => a.IsActive && firstInterval.Overlapping(a.TimeInterval));
        if (overlappende1 >= clinic.Rooms)
            throw new DomainException($"Klinikken har ingen ledige rum til 1. behandling (maks. {clinic.Rooms} samtidige bookinger)");

        var overlappende2 = clinicBookinger.Count(a => a.IsActive && secondInterval.Overlapping(a.TimeInterval));
        if (overlappende2 >= clinic.Rooms)
            throw new DomainException($"Klinikken har ingen ledige rum til 2. behandling (maks. {clinic.Rooms} samtidige bookinger)");

        var patientBookinger = (await _appointmentRepo.GetAllByPatientIdAsync(request.PatientId)).ToList();
        var firstPractBookinger = (await _appointmentRepo.GetAllByPractitionerIdAsync(request.FirstPractitionerId)).ToList();
        var secondPractBookinger = (await _appointmentRepo.GetAllByPractitionerIdAsync(request.SecondPractitionerId)).ToList();

        // Opret første aftale og beregn pris
        var first = Appointment.Create(
            firstInterval,
            request.FirstTreatmentTypeId,
            request.PatientId,
            request.FirstPractitionerId,
            request.ClinicId,
            firstBasePrice,
            patientBookinger,
            firstPractBookinger);

        // Tilføj til lokale lister så anden aftale kan validere mod den
        patientBookinger.Add(first);
        firstPractBookinger.Add(first);
        if (request.FirstPractitionerId == request.SecondPractitionerId)
            secondPractBookinger.Add(first);

        // Opret anden aftale med de opdaterede lister — sammenhængende blokke overlapper ikke
        var second = Appointment.Create(
            secondInterval,
            request.SecondTreatmentTypeId,
            request.PatientId,
            request.SecondPractitionerId,
            request.ClinicId,
            secondBasePrice,
            patientBookinger,
            secondPractBookinger);

        // Beregn priser som én enhed — én rabat på den samlede pris, ikke én rabat per behandling
        var (firstBreakdown, secondBreakdown) = await _pricingService.CalculateCombined(
            request.FirstTreatmentTypeId, request.FirstDurationMinutes,
            request.SecondTreatmentTypeId, request.SecondDurationMinutes,
            request.PatientId, request.From);
        first.ApplyFinalPrice(firstBreakdown.FinalPrice);
        first.ApplyDiscountType(firstBreakdown.BestDiscount?.DiscountType ?? DiscountType.None);
        second.ApplyFinalPrice(secondBreakdown.FinalPrice);
        second.ApplyDiscountType(secondBreakdown.BestDiscount?.DiscountType ?? DiscountType.None);

        await _appointmentRepo.AddAsync(first);
        await _appointmentRepo.AddAsync(second);
        await _appointmentRepo.SaveAsync();
    }
}
