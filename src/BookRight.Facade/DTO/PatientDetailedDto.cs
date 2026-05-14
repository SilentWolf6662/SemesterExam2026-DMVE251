namespace BookRight.Facade.DTO;

public record PatientDetailedDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateTime Birthday,
    string StreetName,
    int Zipcode,
    string Note,
    Guid PreferredPractitionerId);
