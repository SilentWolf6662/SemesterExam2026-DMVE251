namespace BookRight.Facade.Command
{
    public record UpdatePreferredPractitionerRequest(
        Guid PatientId,
        Guid NewPractitionerId);
}
