using BookRight.Domain.Enums;

namespace BookRight.UseCases.Discount;

public record DiscountResult(string DiscountName, decimal DiscountAmount, bool IsApplicable, DiscountType DiscountType);
