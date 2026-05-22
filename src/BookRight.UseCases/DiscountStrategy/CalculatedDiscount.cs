namespace BookRight.Domain.Discount;

public class CalculatedDiscount
{
    public string DiscountType { get; init; } = string.Empty;
    public decimal DiscountAmount { get; init; } = 0;

    // Regn parallelt og lav en race condition der tjekker og overskriver den tidligere pris hvis den nye pris er bedre

    // EF-Core kræver en parameterløs konstruktør
    private CalculatedDiscount() { }

    public CalculatedDiscount(string discountType, decimal discountAmount)
    {
        DiscountType = discountType;
        DiscountAmount = discountAmount;
    }
}
