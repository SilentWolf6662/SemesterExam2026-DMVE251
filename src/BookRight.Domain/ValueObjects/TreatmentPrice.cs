namespace BookRight.Domain.ValueObjects;

// TreatmentPrice er et value object der beskriver et pris valg for en behandlingstype.
// En record bruges fordi to instanser med samme DurationMinutes og BasePrice er ens —
// der er ingen identitet udover selve værdierne (value equality frem for reference equality).
public record TreatmentPrice(int DurationMinutes, decimal BasePrice);