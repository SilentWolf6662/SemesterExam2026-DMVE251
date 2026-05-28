using BookRight.Domain.Enums;

namespace BookRight.Domain.Discount.DiscountStrategy;

public class BronzeLoyaltyDiscountStrategy : IDiscountStrategy
{
    public string Name => "Bronze Loyalitetsrabat";
    private decimal DiscountRate => 0.05m; // 5% rabat

    async Task<DiscountResult> IDiscountStrategy.Calculate(DiscountInput input)
    {
        // Hvis patientens samlede bookingbeløb de sidste 12 måneder er mellem 3.000-10.000 kr., kan der gives rabat
        if (input.PatientBooking12MonthTotalSum >= 3000 && input.PatientBooking12MonthTotalSum <= 10000)
        {
            var discountAmount = input.CurrentPrice * DiscountRate;

            // Returnere en DiscountResult med rabatten og angiver, at den er gyldig
            return new DiscountResult(Name, discountAmount, true, DiscountType.BronzeLoyalty);
        }
        else // ellers retuneres der ingen rabat
        {
            return new DiscountResult(Name, 0, false, DiscountType.None);
        }
    }
}
