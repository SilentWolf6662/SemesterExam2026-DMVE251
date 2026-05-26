namespace BookRight.Domain.Discount.DiscountStrategy;

public class CampaignDiscountStrategy
{
    public string Name { get; set; }
    private decimal DiscountRate { get; set; } = 0;

    public CampaignDiscountStrategy(string name, decimal discountrate)
    {
        Name = name; // "Kampagne Rabat"
        DiscountRate = discountrate;
    }
}
