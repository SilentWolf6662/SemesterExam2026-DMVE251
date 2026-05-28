namespace BookRight.Domain.Discount;

public interface IDiscountStrategy
{
    Task<DiscountResult> Calculate(DiscountInput input);
}