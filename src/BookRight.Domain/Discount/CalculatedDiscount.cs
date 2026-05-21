namespace BookRight.Domain.Discount;

public class CalculatedDiscount
{
    public decimal DiscountAmount { get; init; } = 0;

    // EF-Core kræver en parameterløs konstruktør
    private CalculatedDiscount() { }

    public CalculatedDiscount(decimal discountAmount)
    {
        DiscountAmount = discountAmount;
    }
}
