using BookRight.Domain.Entities;

namespace BookRight.UseCases.Interfaces;

public interface ICampaignRepository
{
    // Henter kampagnen som en appointments sluttid falder ind under
    Task<Campaign?> GetCampaignForAppointmentTimeAsync(DateTime end);
}
