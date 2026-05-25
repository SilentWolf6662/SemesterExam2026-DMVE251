namespace BookRight.Facade.Command;

// Kommando for en kombineret booking — to sammenhængende behandlinger i ét besøg.
// Anden behandling starter præcis når første slutter (FirstDurationMinutes bestemmer snitpunktet).
public record BookCombinedAppointmentRequest(
    DateTime From,
    Guid PatientId,
    Guid FirstPractitionerId,
    Guid FirstTreatmentTypeId,
    int FirstDurationMinutes,
    Guid SecondPractitionerId,
    Guid SecondTreatmentTypeId,
    int SecondDurationMinutes);
