namespace BookRight.Facade.DTO;

public record AppointmentDetailedDto(
    Guid Id,
    DateTime Start,
    DateTime End,
    Guid TreatmentTypeId,
    Guid PatientId,
    Guid PractitionerId,
    string Status,
    string Note);
