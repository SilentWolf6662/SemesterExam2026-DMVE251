using BookRight.Domain.Enums;

namespace BookRight.UseCases.Services;

public record CombinedPriceResult(
    decimal FirstFinalPrice,
    decimal SecondFinalPrice,
    DiscountType DiscountType);
