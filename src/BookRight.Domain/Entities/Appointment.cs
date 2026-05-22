using BookRight.Domain.Enums;
using BookRight.Domain.Exceptions;
using BookRight.Domain.ValueObjects;

namespace BookRight.Domain.Entities;

public class Appointment : AggregateRoot
{
    public TimeInterval TimeInterval { get; private set; }
    public Guid TreatmentTypeId { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid PractitionerId { get; private set; }
    public string Note { get; private set; } = string.Empty;
    public AppointmentStatus Status { get; private set; }
    // Prisen låses fast ved oprettelsen via PricingService og TreatmentType.GetBasePrice().
    // Den gemmes på bookingen så en fremtidig prisændring på behandlingstypen
    // ikke påvirker allerede oprettede bookinger.
    public decimal Price { get; private set; }

    // PRIVAT constructor — tvinger brug af factory-metoden Create()
    private Appointment() { } // EF Core kræver en parameterløs konstruktør
    private Appointment(TimeInterval timeInterval, Guid type, Guid patient, Guid practitioner)
    {
        TimeInterval = timeInterval;
        TreatmentTypeId = type;
        PatientId = patient;
        PractitionerId = practitioner;
        // Alle nye bookinger starter med status Booked
        Status = AppointmentStatus.Booked;
    }

    // ── Factory-metode: eneste måde at oprette en booking for behandling ──────
    public static Appointment Create(
        TimeInterval timeInterval,
        Guid treatmentTypeId,
        Guid patientId,
        Guid practitionerId,
        decimal price,           // Beregnet endeligt beløb inkl. evt. aftens/weekend-tillæg
        IEnumerable<Appointment> existingForPatient,
        IEnumerable<Appointment> existingForPractitioner)
    {
        // Opret den nye booking — status sættes automatisk til Booked i konstruktøren
        var appointment = new Appointment(timeInterval, treatmentTypeId, patientId, practitionerId);

        // Prisen tildeles efter objektet er oprettet fordi Price har private set
        appointment.Price = price;

        // Tjek at den nye booking ikke overlapper med eksisterende aktive bookinger
        // for hverken patienten eller behandleren
        ValidateNoOverlap(appointment, existingForPatient, existingForPractitioner);
        return appointment;
    }

    public void UpdateTreatmentType(Guid newType)
    {
        //Hvis behandlingen er aflyst, gennemført eller Noshow, kan behandlingstypen ikke opdateres
        if (Status == AppointmentStatus.Cancelled || Status == AppointmentStatus.Completed || Status == AppointmentStatus.NoShow)
        {
            throw new DomainException("Kan ikke opdatere behandlingstype siden behandling er afsluttet");
        }
        TreatmentTypeId = newType; // Skift den gamle type til den nye
    }

    // Metode til at markere en appointment som NoShow
    public void NoOneShowed()
    {
        Status = AppointmentStatus.NoShow; // Opdater status til NoShow
    }

    // Metode til at annullere en appointment
    public void Cancel()
    {
        Status = AppointmentStatus.Cancelled; // Opdater status til Cancelled
    }

    // Metode til at markere en appointment som gennemført
    public void Complete(string note)
    {
        Status = AppointmentStatus.Completed; // Opdater status til Completed

        Note = note; // Gem eventuelle noter om behandlingen
    }

    // En appointment har en pris hvis den er større end 0
    public bool HasPrice => Price > 0;

    // Opdaterer prisen med den endelige beregnede pris fra PricingService.
    public void ApplyFinalPrice(decimal finalPrice)
    {
        Price = finalPrice;
    }

    public bool IsActive => Status == AppointmentStatus.Booked; // En appointment er aktiv hvis den er 'Booked' (IKKE 'Cancelled', 'Completed', eller 'NoShow')

    // Metode til at validere at en oprettet appointment ikke overlapper med en eksisterende appoinment
    private static void ValidateNoOverlap(Appointment appointment, IEnumerable<Appointment> existingForPatient, IEnumerable<Appointment> existingForPractitioner)
    {
        var activeForPatient = existingForPatient.Where(k => k.IsActive); // Filtrer kun aktive appointments for patienten
        var activeForPractitioner = existingForPractitioner.Where(k => k.IsActive); // Filtrer kun aktive appointments for behandleren

        // Tjek for overlap med patientens eksisterende appointments
        if (activeForPatient.Any(existingAppointment => appointment.TimeInterval.Overlapping(existingAppointment.TimeInterval)))
        {
            throw new DomainException("Der er overlap mellem en anden behandling for patienten");
        }

        // Tjek for overlap med behandlerens eksisterende appointments
        if (activeForPractitioner.Any(existingAppointment => appointment.TimeInterval.Overlapping(existingAppointment.TimeInterval)))
        {
            throw new DomainException("Der er overlap mellem en anden behandling for behandler");
        }
    }
}