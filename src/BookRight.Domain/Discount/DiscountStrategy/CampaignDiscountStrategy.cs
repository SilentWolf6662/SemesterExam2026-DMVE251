using BookRight.Domain.Enums;

namespace BookRight.Domain.Discount.DiscountStrategy;

public class CampaignDiscountStrategy : IDiscountStrategy
{
    async Task<DiscountResult> IDiscountStrategy.Calculate(DiscountInput input)
    {
        // Hvis patientens samlede bookingbeløb de sidste 12 måneder er mellem 3.000-10.000 kr., kan der gives rabat
        if (input.CampaignDiscountRate > 0)
        {
            decimal discount = input.CurrentPrice * input.CampaignDiscountRate;

            // Returnere en DiscountResult med rabatten og angiver, at den er gyldig
            return new DiscountResult(input.CampaignName, discount, true, DiscountType.Campaign);
        }
        else // ellers retuneres der ingen rabat
        {
            return new DiscountResult(input.CampaignName, 0, false, DiscountType.None);
        }
    }
}
