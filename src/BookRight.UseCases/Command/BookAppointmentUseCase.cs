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
    private readonly PricingService _pricingService;
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly IPractitionerRepository _practitionerRepo;
    private readonly IPatientRepository _patientRepo;
    private readonly ITreatmentTypeRepository _treatmentTypeRepo;
    private readonly IClinicRepository _clinicRepo;

    public BookAppointmentUseCase(IAppointmentRepository appointmentRepo, IPractitionerRepository practitionerRepo, IPatientRepository patientRepo, ITreatmentTypeRepository treatmentTypeRepo, PricingService pricingService, IClinicRepository clinicRepo)
    {
        _appointmentRepo = appointmentRepo;
        _practitionerRepo = practitionerRepo;
        _patientRepo = patientRepo;
        _treatmentTypeRepo = treatmentTypeRepo;
        _pricingService = pricingService;
        _clinicRepo = clinicRepo;
    }

    public async Task Execute(BookAppointmentRequest request)
    {
        // Validér at patient og behandler eksisterer inden vi gør mere.
        // ?? throw stopper udførelsen med det samme hvis entiteten ikke findes.
        _ = await _patientRepo.GetByIdAsync(request.PatientId)
            ?? throw new NotFoundException("Patient not found");

        _ = await _practitionerRepo.GetByIdAsync(request.PractitionerId)
            ?? throw new NotFoundException("Practitioner not found");

        // Klinikken skal eksistere og vi skal bruge antallet af rum til kapacitetstjek
        var clinic = await _clinicRepo.GetByIdAsync(request.ClinicId)
            ?? throw new NotFoundException("Klinik ikke fundet");

        // Vi henter behandlingstypen fordi vi skal bruge dens prisliste til at beregne bookingprisen
        var treatmentType = await _treatmentTypeRepo.GetByIdAsync(request.TreatmentTypeId)
            ?? throw new NotFoundException("Behandlingstype not found");

        var timeInterval = new TimeInterval(request.From, request.To);

        // Trin 1: slå basisprisen op for den valgte varighed (kaster DomainException hvis varighed er ugyldig)
        decimal basePrice = treatmentType.GetBasePrice(request.DurationMinutes);

        // Hent eksisterende bookinger for begge parter så domænet kan tjekke for tidsoverlap
        var patientBookinger = await _appointmentRepo.GetAllByPatientIdAsync(request.PatientId);
        var practitionerBookinger = await _appointmentRepo.GetAllByPractitionerIdAsync(request.PractitionerId);

        // Rumsbegrænsning: tæl aktive bookinger på klinikken der overlapper med det ønskede tidspunkt
        var clinicBookinger = await _appointmentRepo.GetAllByClinicIdAsync(request.ClinicId);
        var overlappende = clinicBookinger.Count(a => a.IsActive && timeInterval.Overlapping(a.TimeInterval));
        if (overlappende >= clinic.Rooms)
            throw new DomainException($"Klinikken har ingen ledige rum på det valgte tidspunkt (maks. {clinic.Rooms} samtidige bookinger)");

        // Opret appointment med basisprisen — validerer overlap og sætter Price så PricingService kan læse den
        var appointment = Appointment.Create(
            timeInterval,
            request.TreatmentTypeId,
            request.PatientId,
            request.PractitionerId,
            request.ClinicId,
            basePrice,
            patientBookinger,
            practitionerBookinger);

        // Trin 2: anvend evt. aftens/weekend-tillæg og rabatter via PricingService
        decimal finalPrice = await _pricingService.Calculate(appointment);
        appointment.ApplyFinalPrice(finalPrice);

        // Gem den nye booking i databasen — AddAsync stager den, SaveAsync sender SQL
        await _appointmentRepo.AddAsync(appointment);
        await _appointmentRepo.SaveAsync();
    }
}