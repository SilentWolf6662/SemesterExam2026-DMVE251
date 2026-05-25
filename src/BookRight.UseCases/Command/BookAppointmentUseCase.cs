using BookRight.Domain.Entities;
using BookRight.Domain.Exceptions;
using BookRight.Domain.ValueObjects;
using BookRight.Facade.Command;
using BookRight.Facade.Interfaces.UseCase;
using BookRight.UseCases.Repositories;
using BookRight.UseCases.Services;

namespace BookRight.UseCases.Command;

public class BookAppointmentUseCase : IBookAppointmentUseCase
{
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly IPractitionerRepository _practitionerRepo;
    private readonly IPatientRepository _patientRepo;
    private readonly ITreatmentTypeRepository _treatmentTypeRepo;

    public BookAppointmentUseCase(IAppointmentRepository appointmentRepo, IPractitionerRepository practitionerRepo, IPatientRepository patientRepo, ITreatmentTypeRepository treatmentTypeRepo)
    {
        _appointmentRepo = appointmentRepo;
        _practitionerRepo = practitionerRepo;
        _patientRepo = patientRepo;
        _treatmentTypeRepo = treatmentTypeRepo;
    }

    public async Task Execute(BookAppointmentRequest request)
    {
        // Validér at patient og behandler eksisterer inden vi gør mere.
        // ?? throw stopper udførelsen med det samme hvis entiteten ikke findes.
        _ = await _patientRepo.GetByIdAsync(request.PatientId)
            ?? throw new NotFoundException("Patient not found");

        _ = await _practitionerRepo.GetByIdAsync(request.PractitionerId)
            ?? throw new NotFoundException("Practitioner not found");

        // Vi henter behandlingstypen fordi vi skal bruge dens prisliste til at beregne bookingprisen
        var treatmentType = await _treatmentTypeRepo.GetByIdAsync(request.TreatmentTypeId)
            ?? throw new NotFoundException("Behandlingstype not found");

        var timeInterval = new TimeInterval(request.From, request.To);

        // Trin 1: slå basisprisen op for den valgte varighed (kaster DomainException hvis varighed er ugyldig)
        decimal basePrice = treatmentType.GetBasePrice(request.DurationMinutes);

        // Hent eksisterende bookinger for begge parter så domænet kan tjekke for tidsoverlap
        var patientBookinger = await _appointmentRepo.GetAllByPatientIdAsync(request.PatientId);
        var practitionerBookinger = await _appointmentRepo.GetAllByPractitionerIdAsync(request.PractitionerId);

        // Opret appointment med basisprisen — validerer overlap og sætter Price så PricingService kan læse den
        var appointment = Appointment.Create(
            timeInterval,
            request.TreatmentTypeId,
            request.PatientId,
            request.PractitionerId,
            basePrice,
            patientBookinger,
            practitionerBookinger);

        // Gem den nye booking i databasen — AddAsync stager den, SaveAsync sender SQL
        await _appointmentRepo.AddAsync(appointment);
        await _appointmentRepo.SaveAsync();
    }
}