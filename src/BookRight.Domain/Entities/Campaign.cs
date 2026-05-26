using BookRight.Domain.Exceptions;
using BookRight.Domain.ValueObjects;

namespace BookRight.Domain.Entities;

public class Campaign : AggregateRoot
{
    public string Name { get; private set; }
    public decimal DiscountRate { get; private set; }
    public TimeInterval TimeInterval { get; private set; }

    private Campaign() { } // EF-Core kræver en parameterløs konstruktør
    private Campaign(string name, decimal discountRate, TimeInterval timeInterval)
    {
        Name = name;
        DiscountRate = discountRate;
        TimeInterval = timeInterval;
    }

    // Factory-metode — eneste måde at oprette en Campaign på udefra
    public static Campaign Create(string name, decimal discountRate, TimeInterval timeInterval, IEnumerable<Campaign> existingCampaigns)
    {
        // Opret den nye kampagne
        var campaign = new Campaign(name, discountRate, timeInterval);
        ValidateNoOverlap(campaign, existingCampaigns);
        return campaign;
    }

    private static void ValidateNoOverlap(Campaign campaign, IEnumerable<Campaign> existingCampaigns)
    {
        var activeCampaigns = existingCampaigns.Where(c => c.TimeInterval.Overlapping(campaign.TimeInterval));
        
        if (activeCampaigns.Any(existingCampaigns => campaign.TimeInterval.Overlapping(existingCampaigns.TimeInterval)))
        {
            throw new DomainException("Der er overlap med en anden kampagne");
        }
    }
}
