using BookRight.Domain.Exceptions;

namespace BookRight.Domain.ValueObjects;

public record TimeInterval
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }

    public TimeInterval(DateTime start, DateTime end)
    {
        // Hvis slut tiden er før start tiden, kastes en DomainException
        if (end <= start) throw new DomainException("Slut tiden kan ikke være før start tiden");

        Start = start;
        End = end;
    }
    // Beregner varigheden af tidsintervallet
    public TimeSpan Duration => End - Start;

    // Hvis Start er mindre end other.End og End er større end other.Start, så overlapper de
    public bool Overlapping(TimeInterval other) => Start < other.End && End > other.Start;
    // Altså ved et tidsinterval på 9:00 - 10:00 vil følgende intervaller overlappe:
    // Interval: 9:30 - 9:45
    // Interval: 8:30 - 9:30
    // Interval: 9:30 - 10:30
}