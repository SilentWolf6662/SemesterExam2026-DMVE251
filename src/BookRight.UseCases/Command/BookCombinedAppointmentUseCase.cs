using BookRight.Domain.Entities;
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
    private readonly PricingService _pricingService;

    public BookCombinedAppointmentUseCase(
        IAppointmentRepository appointmentRepo,
        IPractitionerRepository practitionerRepo,
        IPatientRepository patientRepo,
        ITreatmentTypeRepository treatmentTypeRepo,
        PricingService pricingService)
    {
        _appointmentRepo = appointmentRepo;
        _practitionerRepo = practitionerRepo;
        _patientRepo = patientRepo;
        _treatmentTypeRepo = treatmentTypeRepo;
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

        var patientBookinger = (await _appointmentRepo.GetAllByPatientIdAsync(request.PatientId)).ToList();
        var firstPractBookinger = (await _appointmentRepo.GetAllByPractitionerIdAsync(request.FirstPractitionerId)).ToList();
        var secondPractBookinger = (await _appointmentRepo.GetAllByPractitionerIdAsync(request.SecondPractitionerId)).ToList();

        // Opret første aftale og beregn pris
        var first = Appointment.Create(
            firstInterval,
            request.FirstTreatmentTypeId,
            request.PatientId,
            request.FirstPractitionerId,
            firstBasePrice,
            patientBookinger,
            firstPractBookinger);

        first.ApplyFinalPrice(await _pricingService.Calculate(first));

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
            secondBasePrice,
            patientBookinger,
            secondPractBookinger);

        second.ApplyFinalPrice(await _pricingService.Calculate(second));

        await _appointmentRepo.AddAsync(first);
        await _appointmentRepo.AddAsync(second);
        await _appointmentRepo.SaveAsync();
    }
}
