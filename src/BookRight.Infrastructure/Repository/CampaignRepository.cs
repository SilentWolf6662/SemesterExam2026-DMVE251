using BookRight.Domain.Entities;
using BookRight.UseCases.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookRight.Infrastructure.Repository;

public class CampaignRepository : ICampaignRepository
{
    private readonly AppDbContext _db;

    public CampaignRepository(AppDbContext db)
    {
        _db = db;
    }

    // Henter kampagnen som en appointments sluttid falder ind under
    async Task<Campaign?> ICampaignRepository.GetCampaignForAppointmentTimeAsync(DateTime end)
    {
        return await _db.Campaigns
            .AsNoTracking()
            // Kun kampagnen, hvor en appointments sluttid falder inden for kampagnens tidsinterval
            .Where(c => c.TimeInterval.Start <= end && c.TimeInterval.End >= end)
            .FirstOrDefaultAsync(); // Tag den første/default kampagne, siden der ikke er overlap pga. validering ved oprettelse
    }
}
