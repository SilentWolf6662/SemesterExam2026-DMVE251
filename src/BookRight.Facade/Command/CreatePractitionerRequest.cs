namespace BookRight.Facade.Command;

public record CreatePractitionerRequest(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    string Authorization,
    int AuthorizationNumber);