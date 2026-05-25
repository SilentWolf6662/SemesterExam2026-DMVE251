namespace BookRight.Facade.DTO;

public record PricePreviewDto(
    decimal BasePrice,
    decimal OvertimeSurcharge,
    decimal PriceBeforeDiscount,
    string DiscountName,
    decimal DiscountAmount,
    string DiscountType,
    decimal FinalPrice);
