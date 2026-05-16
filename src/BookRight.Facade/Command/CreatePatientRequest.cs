namespace BookRight.Facade.Command;

public record CreatePatientRequest(string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateTime Birthday,
    string StreetName,
    int Zipcode,
    string Note,
    Guid? PreferredPractitioner);