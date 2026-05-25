using BookRight.Domain.Enums;
using BookRight.UseCases.Discount;

namespace BookRight.Domain.Discount.DiscountStrategy;

public class SilverLoyaltyDiscountStrategy : IDiscountStrategy
{
    public string Name => "Sølv Loyalitetsrabat";
    private decimal DiscountRate => 0.1m; // 10% rabat

    async Task<DiscountResult> IDiscountStrategy.Calculate(DiscountInput input)
    {
        // Hvis patientens samlede bookingbeløb de sidste 12 måneder er mellem 10.001-25.000 kr., kan der gives rabat
        if (input.PatientBooking12MonthTotalSum >= 10001 && input.PatientBooking12MonthTotalSum <= 25000)
        {
            var discountAmount = input.CurrentPrice * DiscountRate;
            return new DiscountResult(Name, discountAmount, true, DiscountType.SilverLoyalty);
        }
        else // ellers retuneres der ingen rabat
        {
            return new DiscountResult(Name, 0, false, DiscountType.None);
        }
    }
}
