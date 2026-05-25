namespace BookRight.Facade.Command;

// BookAppointmentRequest er den kommando som Blazor sender til BookAppointmentUseCase.
// DurationMinutes bruges til at slå den korrekte basispris op i TreatmentType.Prices —
// samme behandlingstype kan have forskellige priser afhængigt af varighed (fx 30, 45 eller 60 min).
public record BookAppointmentRequest(DateTime From, DateTime To, int DurationMinutes, Guid TreatmentTypeId, Guid PatientId, Guid PractitionerId, Guid ClinicId);
