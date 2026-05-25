using BookRight.Domain.ValueObjects;

namespace BookRight.Domain.Entities;

public class Patient : AggregateRoot
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string PhoneNumber { get; private set; }
    public string Email { get; private set; }
    public DateTime Birthday { get; private set; }
    public Address PatientAddress { get; private set; }
    public string? Note { get; private set; }
    public Guid PreferredPractitioner { get; private set; } // Navigational Property

    // PRIVAT constructor — tvinger brug af factory-metoden Create()
    private Patient() { } // EF Core
    private Patient(string firstName, string lastName, string phoneNumber, string email, DateTime birthDate, Address address, string? note, Guid preferredPractitioner) 
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Email = email;
        Birthday = birthDate;
        PatientAddress = address;
        Note = note;
        PreferredPractitioner = preferredPractitioner;
    }

    public void UpdatePreferredPractitioner(Guid newPractitionerId)
    {
        PreferredPractitioner = newPractitionerId;
    }

    // ── Factory-metode: eneste måde at oprette en patient / kunde ──────
    public static Patient Create(string firstName, string lastName, string phoneNumber, string email, DateTime birthDate, Address address, string? note, Guid preferredPractitioner)
    {
        // Laver og retuner en ny patient med et fornavn, efternavn, email, fødselsdag, adresse, note og foretrukket behandler
        return new Patient(firstName, lastName, phoneNumber, email, birthDate, address, note, preferredPractitioner); ;
    }
}