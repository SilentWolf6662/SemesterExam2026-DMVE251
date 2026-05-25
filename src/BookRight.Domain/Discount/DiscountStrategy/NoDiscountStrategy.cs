using BookRight.Domain.Enums;
using BookRight.UseCases.Discount;

namespace BookRight.UseCases.Discount.DiscountStrategy;

public class NoDiscountStrategy : IDiscountStrategy
{
    public string Name => "Ingen rabat";

    async Task<DiscountResult> IDiscountStrategy.Calculate(DiscountInput input)
    {
        // Hvis der ikke kan gives nogen rabat vil denne metode altid være default,
        // siden at den er appliable og den giver en default rabat på 0, så der ikke er en blank UI.
        return new DiscountResult(Name, 0, true, DiscountType.None);
    }
}
