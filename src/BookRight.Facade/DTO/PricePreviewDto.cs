namespace BookRight.Facade.DTO;

// DTO der sendes fra Infrastructure til UI-laget med det fulde prisregnskab for én booking.
// Bruges i BookAppointment til at vise priskortet inden brugeren opretter bookingen.
public record PricePreviewDto(
    decimal BasePrice,           // Basisprisen fra behandlingstypens prisliste
    decimal OvertimeSurcharge,   // 15% tillæg hvis aftenen (≥17:00) eller weekend, ellers 0
    decimal PriceBeforeDiscount, // Prisen efter overtidstillæg men før rabat
    string DiscountName,         // Navn på den anvendte rabat, fx "Bronze Loyalitetsrabat"
    decimal DiscountAmount,      // Rabatbeløbet i kroner, 0 hvis ingen rabat
    string DiscountType,         // Rabattypen som tekst, fx "BronzeLoyalty" eller "None"
    decimal FinalPrice);         // Den endelige pris kunden betaler
