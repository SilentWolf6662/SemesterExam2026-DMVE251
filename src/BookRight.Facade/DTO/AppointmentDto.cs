namespace BookRight.Facade.DTO;

// AppointmentDto bruges til kalendervisning og lister.
// Price er den endelige pris der blev beregnet da bookingen blev oprettet,
// inklusiv evt. aftens/weekend-tillæg — ikke en live-beregning.
public record AppointmentDto(
    Guid Id,
    DateTime Start,
    DateTime End,
    Guid TreatmentTypeId,
    Guid PractitionerId,
    string Status,
    decimal Price);
