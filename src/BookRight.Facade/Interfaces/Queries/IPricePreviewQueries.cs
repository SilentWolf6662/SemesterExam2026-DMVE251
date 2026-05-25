using BookRight.Facade.DTO;

namespace BookRight.Facade.Interfaces.Queries;

public interface IPricePreviewQueries
{
    Task<PricePreviewDto> GetPreviewAsync(Guid treatmentTypeId, int durationMinutes, Guid patientId, DateTime from);

    Task<(PricePreviewDto First, PricePreviewDto Second)> GetCombinedPreviewAsync(
        Guid treatment1Id, int duration1,
        Guid treatment2Id, int duration2,
        Guid patientId, DateTime from);
}
