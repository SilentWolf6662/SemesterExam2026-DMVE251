namespace BookRight.Facade.Command;

public record ChangeStatusRequest(Guid AppointmentId, string Note, string Status);