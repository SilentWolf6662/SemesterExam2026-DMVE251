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

    // Henter prisestimering for én enkelt booking og konverterer til DTO
    public async Task<PricePreviewDto> GetPreviewAsync(Guid treatmentTypeId, int durationMinutes, Guid patientId, DateTime from)
    {
        var b = await _pricingService.Calculate(treatmentTypeId, durationMinutes, patientId, from);
        return Map(b);
    }

    // Henter prisestimering for en kombineret booking — kalder CalculateCombined
    // der beregner én rabat på den samlede pris og fordeler den proportionalt
    public async Task<(PricePreviewDto First, PricePreviewDto Second)> GetCombinedPreviewAsync(
        Guid treatment1Id, int duration1,
        Guid treatment2Id, int duration2,
        Guid patientId, DateTime from)
    {
        var (first, second) = await _pricingService.CalculateCombined(
            treatment1Id, duration1, treatment2Id, duration2, patientId, from);
        return (Map(first), Map(second));
    }

    // Konverterer et PriceBreakdown til en PricePreviewDto der kan sendes til UI-laget.
    // ?? sikrer at null-værdier erstattes med standardtekster/0 så UI ikke fejler.
    private static PricePreviewDto Map(PriceBreakdown b) => new(
        b.BasePrice,
        b.OvertimeSurcharge,
        b.PriceBeforeDiscount,
        b.BestDiscount?.DiscountName ?? "Ingen rabat",
        b.BestDiscount?.DiscountAmount ?? 0,
        b.BestDiscount?.DiscountType.ToString() ?? "None",
        b.FinalPrice);
}
