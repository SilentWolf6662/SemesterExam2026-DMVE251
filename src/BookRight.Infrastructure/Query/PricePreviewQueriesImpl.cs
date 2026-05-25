using BookRight.Facade.DTO;
using BookRight.Facade.Interfaces.Queries;
using BookRight.UseCases.Services;

namespace BookRight.Infrastructure.Query;

public class PricePreviewQueriesImpl : IPricePreviewQueries
{
    private readonly PricingService _pricingService;

    public PricePreviewQueriesImpl(PricingService pricingService)
    {
        _pricingService = pricingService;
    }

    public async Task<PricePreviewDto> GetPreviewAsync(Guid treatmentTypeId, int durationMinutes, Guid patientId, DateTime from)
    {
        var b = await _pricingService.CalculatePreview(treatmentTypeId, durationMinutes, patientId, from);
        return Map(b);
    }

    public async Task<(PricePreviewDto First, PricePreviewDto Second)> GetCombinedPreviewAsync(
        Guid treatment1Id, int duration1,
        Guid treatment2Id, int duration2,
        Guid patientId, DateTime from)
    {
        var (first, second) = await _pricingService.CalculateCombinedPreview(
            treatment1Id, duration1, treatment2Id, duration2, patientId, from);
        return (Map(first), Map(second));
    }

    private static PricePreviewDto Map(PriceBreakdown b) => new(
        b.BasePrice,
        b.OvertimeSurcharge,
        b.PriceBeforeDiscount,
        b.BestDiscount?.DiscountName ?? "Ingen rabat",
        b.BestDiscount?.DiscountAmount ?? 0,
        b.BestDiscount?.DiscountType.ToString() ?? "None",
        b.FinalPrice);
}
