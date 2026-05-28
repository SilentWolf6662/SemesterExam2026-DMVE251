namespace BookRight.Domain.Entities;

public class OvertimeCharge
{
    private const decimal Surcharge = 0.15m; // BookRight-reglerne specificerer 15 % tillæg på aftener og weekender
    private static readonly TimeOnly EveningStart = new TimeOnly(17, 0); // Aftenstillæg gælder fra kl. 17:00 og frem

    // Returnerer basispris + 15 % hvis bookingen er om aftenen (fra 17:00) eller i weekenden.
    public static decimal Calculate(DateTime start, decimal basePrice)
    {
        // Hvis aftalen starter kl. 17 eller senere, pålægges et tillæg
        bool isEvening = TimeOnly.FromDateTime(start) >= EveningStart;
        // Hvis aftalen starter på en lørdag eller søndag, pålægges et tillæg
        bool isWeekend = start.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

        decimal finalPrice = basePrice;
        // Hvis det er aften eller weekend, pålægges et tillæg. Ellers returneres basisprisen uændret.
        if (isEvening || isWeekend)
        {
            finalPrice = finalPrice * (1 + Surcharge);
            // Math.Round(..., 2) sikrer at prisen altid har præcis 2 decimaler (øre-præcision).
            finalPrice = Math.Round(finalPrice, 2);
        }

        return finalPrice;
    }
}
