using BookRight.UseCases.Discount;

namespace BookRight.UseCases.Services;

// Indeholder det fulde prisregnskab for én booking.
// BestDiscount er null hvis ingen rabat er gældende — tjek altid med ?. inden brug.
public record PriceBreakdown(
    decimal BasePrice,           // Basisprisen fra behandlingstypens prisliste
    decimal OvertimeSurcharge,   // Tillæg hvis bookingen er om aftenen (≥17:00) eller i weekenden
    decimal PriceBeforeDiscount, // BasePrice + OvertimeSurcharge — prisen før rabat
    DiscountResult? BestDiscount, // Den bedste fundne rabat, eller null hvis ingen rabat
    decimal FinalPrice);          // Den endelige pris kunden betaler
