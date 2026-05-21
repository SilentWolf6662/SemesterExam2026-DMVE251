using BookRight.Domain.Discount;
using BookRight.Domain.Entities;
using BookRight.Domain.Exceptions;
using BookRight.Domain.ValueObjects;

namespace BookRight.UseCases.Services;

// PricingService er en domæne-service til prisberegning.
// Den er statisk fordi den ikke har nogen tilstand.
// Den transformer blot en indgangsværdi til en udgangsværdi baseret på faste forretningsregler.
public class PricingService
{
    private readonly IEnumerable<IDiscountStrategy> _discountStrategies;

    public PricingService(IEnumerable<IDiscountStrategy> discountStrategies)
    {
        _discountStrategies = discountStrategies;
    }

    // Beregner den endelige pris for en booking.
    
    public decimal Calculate(Appointment appointment, TimeInterval timeInterval)
    {
        // Hvis bookingen ikke har en pris defineret, kastes en Exception
        if (!appointment.HasPrice) throw new DomainException("Appointment skal have en pris for at kunne beregnes.");

        // Beregn basisprisen baseret på BookRight-reglerne, ved at ligge overtidsgebyret oveni,
        // hvis bookingen er om aftenen eller i weekenden
        decimal currentPrice = OvertimeCharge.Calculate(appointment, timeInterval);

        // Hent alle rabatter fra de forskellige strategier og find den bedste (højeste) rabat.
        var discounts = _discountStrategies.Select(a => a.Calculate(appointment, timeInterval));
        var bestDiscount = discounts.MaxBy(a => a.DiscountAmount) ?? new CalculatedDiscount(0);

        // Beregn den endelige pris ved at trække den højeste rabat fra basisprisen
        decimal finalPrice = currentPrice - bestDiscount.DiscountAmount;

        return finalPrice;
    }
}
