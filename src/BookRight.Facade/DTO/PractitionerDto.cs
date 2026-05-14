namespace BookRight.Facade.DTO;

public record PractitionerDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Authorization,
    int AuthorizationNumber,
    List<Guid> ClinicIds);
