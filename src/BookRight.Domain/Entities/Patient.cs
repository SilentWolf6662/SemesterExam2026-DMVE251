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
    public string Note { get; private set; }
    public Guid PreferredPractitioner { get; private set; }
    private Patient() { }
    private Patient(string firstName, string lastName, string phoneNumber, string email, DateTime birthDate, Address address, string note, Guid preferredPractitioner) 
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Email = email;
        Birthday = birthDate;
        Note = note;
        PreferredPractitioner = preferredPractitioner;
    }
    public static Patient create(string firstName, string lastName, string phoneNumber, string email, DateTime birthDate, Address address, string note, Guid preferredPractitioner)
    {   
        return new Patient(firstName, lastName, phoneNumber, email, birthDate, address, note, preferredPractitioner); ;
    }
}