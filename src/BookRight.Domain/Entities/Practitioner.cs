using BookRight.Domain.Enums;

namespace BookRight.Domain.Entities;

public class Practitioner : AggregateRoot
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string PhoneNumber { get; private set; }
    public string Email { get; private set; }
    public AuthorizationType Authorization { get; private set; }
    public int AuthorizationNumber { get; private set; }

    // Private backing-felter — EF Core tilgår dem direkte via feltnavn-konventionen.
    // IReadOnlyList eksponeres udadtil så ingen udefra kan kalde .Add() eller .Remove()
    // direkte på listen — al mutation sker gennem domænemetoderne nedenfor.
    private readonly List<Guid> _clinics = [];
    private readonly List<Guid> _appointments = [];

    public IReadOnlyList<Guid> Clinics => _clinics;
    public IReadOnlyList<Guid> Appointments => _appointments;

    // PRIVAT constructor — tvinger brug af factory-metoden Create()
    private Practitioner() { } // EF Core
    private Practitioner(string firstName, string lastName, string phoneNumber, string email, AuthorizationType authorization, int authorizationNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Email = email;
        Authorization = authorization;
        AuthorizationNumber = authorizationNumber;
    }

    // ── Factory-metode: eneste måde at oprette en behandler ──────
    public static Practitioner Create(string firstName, string lastName, string phoneNumber, string email, AuthorizationType authorization, int authorizationNumber)
    {
        return new Practitioner(firstName, lastName, phoneNumber, email, authorization, authorizationNumber);
    }

    // ── Domænemetoder: eneste måde at mutere listerne på ──────
    public void AddClinic(Guid clinicId)
    {
        if (!_clinics.Contains(clinicId))
            _clinics.Add(clinicId);
    }

    public void AddAppointment(Guid appointmentId)
    {
        if (!_appointments.Contains(appointmentId))
            _appointments.Add(appointmentId);
    }
}
