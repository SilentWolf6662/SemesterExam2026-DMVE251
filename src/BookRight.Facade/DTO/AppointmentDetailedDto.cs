namespace BookRight.Facade.DTO;

// AppointmentDetailedDto bruges i AppointmentModal og overalt hvor alle detaljer om
// én bestemt booking skal vises — inkl. patient, note og den låste pris.
// Adskilt fra AppointmentDto fordi vi ikke henter PatientId og Note til kalendervisning.
public record AppointmentDetailedDto(
    Guid Id,
    DateTime Start,
    DateTime End,
    Guid TreatmentTypeId,
    Guid PatientId,
    Guid PractitionerId,
    string Status,
    string Note,
    decimal Price,
    Guid? CombinedBookingId = null);
