namespace BookRight.Facade.Command;

public record BookAppointmentRequest(DateTime From, DateTime To, Guid TreatmentTypeId, Guid PatientId, Guid PractitionerId);
