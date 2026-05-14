namespace BookRight.Facade.DTO;

public record PatientDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateTime Birthday);
