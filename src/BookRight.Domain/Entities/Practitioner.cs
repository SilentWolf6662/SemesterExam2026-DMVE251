using BookRight.Domain.Enums;

namespace BookRight.Domain.Entities;

public class Practitioner : AggregateRoot
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string PhoneNumber { get; private set; }
    public string Email { get; private set; }
    public AuthorizationType Authorization { get; private set; }
    public List<Guid> Clinics { get; private set; }
    public List<Guid> Appointments { get; private set; }

    // PRIVAT constructor — tvinger brug af factory-metoden Create()
    private Practitioner() { } // EF Core
    private Practitioner(string firstName, string lastName, string phoneNumber, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Email = email;
        Clinics = new List<Guid>();
        Appointments = new List<Guid>();
    }

    // ── Factory-metode: eneste måde at oprette en behandler ──────
    public static Practitioner Create(string firstName, string lastName, string phoneNumber, string email)
    {
        return new Practitioner(firstName, lastName, phoneNumber, email);
    }
}