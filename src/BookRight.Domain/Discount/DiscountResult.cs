using BookRight.Domain.Enums;

namespace BookRight.Domain.Discount;

public record DiscountResult(string DiscountName, decimal DiscountAmount, bool IsApplicable, DiscountType DiscountType);
