namespace BookRight.Domain.Services;

// PricingService er en domæne-service til prisberegning.
// Den er statisk fordi den ikke har nogen tilstand — den transformer blot en indgangsværdi
// til en udgangsværdi baseret på faste forretningsregler.
public static class PricingService
{
    // BookRight-reglerne specificerer 15 % tillæg på aftener og weekender
    private const decimal Surcharge = 0.15m;

    // Aftenstillæg gælder fra kl. 17:00 og frem
    private static readonly TimeOnly EveningStart = new TimeOnly(17, 0);

    // Beregner den endelige pris for en booking.
    // Returnerer basisprisen uændret hvis bookingen er på en hverdag inden kl. 17.
    // Returnerer basispris + 15 % hvis bookingen er om aftenen (fra 17:00) eller i weekenden.
    // Math.Round(..., 2) sikrer at prisen altid har præcis 2 decimaler (øre-præcision).
    public static decimal Calculate(decimal basePrice, DateTime appointmentStart)
    {
        bool isEvening = TimeOnly.FromDateTime(appointmentStart) >= EveningStart;
        bool isWeekend = appointmentStart.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

        return (isEvening || isWeekend) ? Math.Round(basePrice * (1 + Surcharge), 2) : basePrice;
    }
}