using BookRight.Domain.Enums;
using BookRight.UseCases.Discount;

namespace BookRight.Domain.Discount.DiscountStrategy;

public class GoldLoyaltyDiscountStrategy : IDiscountStrategy
{
    public string Name => "Guld Loyalitetsrabat";
    private decimal DiscountRate => 0.15m; // 15% rabat

    async Task<DiscountResult> IDiscountStrategy.Calculate(DiscountInput input)
    {
        // Hvis patientens samlede bookingbeløb de sidste 12 måneder er over 25.000 kr., kan der gives rabat
        if (input.PatientBooking12MonthTotalSum > 25000)
        {
            var discountAmount = input.CurrentPrice * DiscountRate;
            return new DiscountResult(Name, discountAmount, true, DiscountType.GoldLoyalty);
        }
        else // ellers retuneres der ingen rabat
        {
            return new DiscountResult(Name, 0, false, DiscountType.None);
        }
    }
}
