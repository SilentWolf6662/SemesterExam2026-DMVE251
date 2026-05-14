namespace BookRight.Facade.DTO;

public record AppointmentDto(
    Guid Id,
    DateTime Start,
    DateTime End,
    Guid TreatmentTypeId,
    Guid PractitionerId,
    string Status);
