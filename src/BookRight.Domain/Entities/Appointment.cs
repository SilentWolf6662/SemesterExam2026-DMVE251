using BookRight.Domain.Enums;
using BookRight.Domain.Exceptions;
using BookRight.Domain.ValueObjects;

namespace BookRight.Domain.Entities;

public class Appointment : AggregateRoot
{
    public AppointmentTime AppointmentTime { get; private set; }
    public Guid TreatmentTypeId { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid PractitionerId { get; private set; }
    public string Note { get; private set; } = string.Empty;
    public AppointmentStatus Status { get; private set; }
    
    // PRIVAT constructor — tvinger brug af factory-metoden Create()
    private Appointment() { } // EF Core
    private Appointment(AppointmentTime appointmentTime, Guid type, Guid patient, Guid practitioner)
    {

        AppointmentTime = appointmentTime;
        TreatmentTypeId = type;
        PatientId = patient;
        PractitionerId = practitioner;
        Status = AppointmentStatus.Booked;

    }

    // ── Factory-metode: eneste måde at oprette en booking for behandling ──────
    public static Appointment Create(
        AppointmentTime appointmentTime,
        Guid treatmentTypeId,
        Guid patientId,
        Guid practitionerId,
        IEnumerable<Appointment> existingForPatient,
        IEnumerable<Appointment> existingForPractitioner)
    {
        // Laver en ny appointment med en tid, Id for behandlingstype, patient Id og behandler Id
        var appointment = new Appointment(appointmentTime, treatmentTypeId, patientId, practitionerId);
        // Tjek overlap mellem ny appointment med eksisterende appointment, ved at kigge på den nye appointments sluttid og starttid ligger inde i tiden for den eksisterende appointment
        ValidateNoOverlap(appointment, existingForPatient, existingForPractitioner);
        return appointment; // Returner den validerede appointment uden overlap
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
    public void Complete()
    {
        Status = AppointmentStatus.Completed; // Opdater status til Completed
    }

    public bool IsActive => Status == AppointmentStatus.Booked; // En appointment er aktiv hvis den er 'Booked' (IKKE 'Cancelled', 'Completed', eller 'NoShow')

    // Metode til at validere at en oprettet appointment ikke overlapper med en eksisterende appoinment
    private static void ValidateNoOverlap(Appointment appointment, IEnumerable<Appointment> existingForPatient, IEnumerable<Appointment> existingForPractitioner)
    {
        var activeForPatient = existingForPatient.Where(k => k.IsActive); // Filtrer kun aktive appointments for patienten
        var activeForPractitioner = existingForPractitioner.Where(k => k.IsActive); // Filtrer kun aktive appointments for behandleren

        // Tjek for overlap med patientens eksisterende appointments
        if (activeForPatient.Any(existingAppointment => appointment.AppointmentTime.Overlapping(existingAppointment.AppointmentTime)))
        {
            throw new DomainException("Der er overlap mellem en anden behandling for patienten");
        }

        // Tjek for overlap med behandlerens eksisterende appointments
        if (activeForPractitioner.Any(existingAppointment => appointment.AppointmentTime.Overlapping(existingAppointment.AppointmentTime)))
        {
            throw new DomainException("Der er overlap mellem en anden behandling for behandler");
        }
    }
}