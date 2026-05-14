namespace BookRight.Facade.DTO;

public record TreatmentTypeDto(
    Guid Id,
    string Name,
    string AuthorizationType,
    int? MaxParticipants);
