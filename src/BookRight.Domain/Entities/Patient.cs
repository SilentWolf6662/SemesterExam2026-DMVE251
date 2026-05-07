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
    public Guid PreferedPractitioner { get; private set; }
}