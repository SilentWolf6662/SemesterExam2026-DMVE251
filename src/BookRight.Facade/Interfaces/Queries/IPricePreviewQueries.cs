using BookRight.Facade.DTO;

namespace BookRight.Facade.Interfaces.Queries;

public interface IPricePreviewQueries
{
    // Beregner prisestimering for én enkelt booking.
    // Returnerer fuldt breakdown med basispris, overtidstillæg, rabat og endelig pris.
    Task<PricePreviewDto> GetPreviewAsync(Guid treatmentTypeId, int durationMinutes, Guid patientId, DateTime from);

    // Beregner prisestimering for en kombineret booking (to behandlinger i ét besøg).
    // Rabatten beregnes på den samlede pris og fordeles proportionalt — rabat bruges kun én gang.
    // 2. behandling starter præcis når 1. behandling slutter (from + duration1).
    Task<(PricePreviewDto First, PricePreviewDto Second)> GetCombinedPreviewAsync(
        Guid treatment1Id, int duration1,
        Guid treatment2Id, int duration2,
        Guid patientId, DateTime from);
}
