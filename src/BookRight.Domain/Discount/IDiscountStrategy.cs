namespace BookRight.UseCases.Discount;

public interface IDiscountStrategy
{
    Task<DiscountResult> Calculate(DiscountInput input);
}