namespace BookRight.Facade.Command;

public record CompleteAppointmentRequest(Guid AppointmentId, string Note);