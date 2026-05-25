using BookRight.Domain.Enums;

namespace BookRight.UseCases.Services;

// Resultatet af prisberegning for en kombineret booking (to behandlinger i ét besøg).
// Rabatten er beregnet på den samlede pris og fordelt proportionalt mellem de to behandlinger,
// så den samme rabat ikke kan bruges to gange i ét besøg.
public record CombinedPriceResult(
    decimal FirstFinalPrice,   // Endelig pris for 1. behandling efter proportional rabat
    decimal SecondFinalPrice,  // Endelig pris for 2. behandling efter proportional rabat
    DiscountType DiscountType); // Rabattypen der blev anvendt (bruges til at gemme på begge aftaler)
