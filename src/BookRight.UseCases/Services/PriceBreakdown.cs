using BookRight.UseCases.Discount;

namespace BookRight.UseCases.Services;

public record PriceBreakdown(
    decimal BasePrice,
    decimal OvertimeSurcharge,
    decimal PriceBeforeDiscount,
    DiscountResult? BestDiscount,
    decimal FinalPrice);
