using BookRight.Domain.Exceptions;
using BookRight.Domain.ValueObjects;

namespace BookRight.Domain.Tests.ValueObjects;

/// <summary>
/// Tests for TimeInterval-value-objektet.
///
/// Dækker:
/// - Konstruktør-validering
/// - Varighed-beregning
/// - Overlap-logik
/// - Record value equality
/// </summary>
public class TimeIntervalTests
{
    // ── Hjælpe-metoder ────────────────────────────────────────────────────────

    /// <summary>
    /// Opretter et TimeInterval på en fast testdato mellem de angivne timer.
    /// </summary>
    /// <param name="startHour">Starttidspunktets time.</param>
    /// <param name="endHour">Sluttidspunktets time.</param>
    /// <returns>Et nyt TimeInterval.</returns>
    private static TimeInterval CreateInterval(int startHour, int endHour) =>
        new(new DateTime(2026, 1, 1, startHour, 0, 0), new DateTime(2026, 1, 1, endHour, 0, 0));

    /// <summary>
    /// Opretter et TimeInterval på en fast testdato
    /// med timer og minutter.
    /// </summary>
    /// <param name="startHour">Starttidspunktets time.</param>
    /// <param name="startMinute">Starttidspunktets minut.</param>
    /// <param name="endHour">Sluttidspunktets time.</param>
    /// <param name="endMinute">Sluttidspunktets minut.</param>
    /// <returns>Et nyt TimeInterval.</returns>
    private static TimeInterval CreateInterval(int startHour, int startMinute, int endHour, int endMinute) => 
        new(new DateTime(2026, 1, 1, startHour, startMinute, 0), new DateTime(2026, 1, 1, endHour, endMinute, 0));

    // ── Konstruktør-validering ────────────────────────────────────────────────

    /// <summary>
    /// Verificerer at Start og End sættes korrekt
    /// ved oprettelse af et gyldigt TimeInterval.
    /// </summary>
    [Fact]
    public void Constructor_ValidDates_SetsStartAndEndCorrectly()
    {
        // Arrange
        var start = new DateTime(2026, 1, 1, 9, 0, 0);
        var end = new DateTime(2026, 1, 1, 10, 0, 0);

        // Act
        var interval = new TimeInterval(start, end);

        // Assert
        Assert.Equal(start, interval.Start);
        Assert.Equal(end, interval.End);
    }

    /// <summary>
    /// Verificerer at konstruktøren kaster DomainException
    /// når sluttidspunktet ligger før starttidspunktet.
    /// </summary>
    [Fact]
    public void Constructor_EndBeforeStart_ThrowsDomainException()
    {
        // Arrange
        var start = new DateTime(2026, 1, 1, 10, 0, 0);
        var end = new DateTime(2026, 1, 1, 9, 0, 0);

        // Act & Assert
        Assert.Throws<DomainException>(() => new TimeInterval(start, end));
    }

    /// <summary>
    /// Verificerer at konstruktøren kaster DomainException
    /// når start- og sluttidspunkt er identiske.
    /// </summary>
    [Fact]
    public void Constructor_StartEqualsEnd_ThrowsDomainException()
    {
        // Arrange
        var timestamp = new DateTime(2026, 1, 1, 9, 0, 0);

        // Act & Assert
        Assert.Throws<DomainException>(() => new TimeInterval(timestamp, timestamp));
    }

    /// <summary>
    /// Verificerer at et TimeInterval med præcis ét minuts varighed
    /// accepteres som gyldigt.
    /// </summary>
    [Fact]
    public void Constructor_OneMinuteDuration_CreatesValidInterval()
    {
        // Arrange
        var start = new DateTime(2026, 1, 1, 9, 0, 0);
        var end = new DateTime(2026, 1, 1, 9, 1, 0);

        // Act
        var interval = new TimeInterval(start, end);

        // Assert
        Assert.Equal(TimeSpan.FromMinutes(1), interval.Duration);
    }

    // ── Varighed ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Verificerer at Duration returnerer én time
    /// for et interval mellem 09:00 og 10:00.
    /// </summary>
    [Fact]
    public void Duration_OneHourInterval_ReturnsOneHour()
    {
        // Arrange & Act
        var interval = CreateInterval(9, 10);

        // Assert
        Assert.Equal(TimeSpan.FromHours(1), interval.Duration);
    }

    /// <summary>
    /// Verificerer at Duration returnerer to timer
    /// for et interval mellem 08:00 og 10:00.
    /// </summary>
    [Fact]
    public void Duration_TwoHourInterval_ReturnsTwoHours()
    {
        // Arrange & Act
        var interval = CreateInterval(8, 10);

        // Assert
        Assert.Equal(TimeSpan.FromHours(2), interval.Duration);
    }

    // ── Overlap-logik ─────────────────────────────────────────────────────────

    /// <summary>
    /// Verificerer overlap-logik med intervaller der anvender minutter.
    ///
    /// Standard-intervallet er 09:00 - 10:00.
    /// </summary>
    /// <param name="startHour">Sammenlignings-intervallets starttime.</param>
    /// <param name="startMinute">Sammenlignings-intervallets startminut.</param>
    /// <param name="endHour">Sammenlignings-intervallets sluttime.</param>
    /// <param name="endMinute">Sammenlignings-intervallets slutminut.</param>
    /// <param name="expectedResult">Forventet overlap-resultat.</param>
    [Theory]
    [InlineData(9, 30, 9, 45, true)]
    [InlineData(8, 30, 9, 30, true)]
    [InlineData(9, 30, 10, 30, true)]
    public void Overlapping_MinuteBasedIntervals_ReturnsExpectedResult(int startHour, int startMinute, int endHour, int endMinute, bool expectedResult)
    {
        // Arrange
        var defaultInterval = CreateInterval(9, 10);

        var comparisonInterval = CreateInterval(startHour, startMinute, endHour, endMinute);

        // Act
        var result = defaultInterval.Overlapping(comparisonInterval);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    /// <summary>
    /// Verificerer at overlap-logikken er symmetrisk
    /// i begge retninger.
    /// </summary>
    [Fact]
    public void Overlapping_IsSymmetric_ReturnsTrueBothWays()
    {
        // Arrange
        var first = CreateInterval(9, 11);
        var second = CreateInterval(10, 12);

        // Act & Assert
        Assert.True(first.Overlapping(second));
        Assert.True(second.Overlapping(first));
    }

    /// <summary>
    /// Verificerer at et interval der er fuldt indeholdt
    /// i et andet interval betragtes som overlap.
    /// </summary>
    [Fact]
    public void Overlapping_ContainedInterval_ReturnsTrue()
    {
        // Arrange
        var outer = CreateInterval(9, 12);
        var inner = CreateInterval(10, 11);

        // Act & Assert
        Assert.True(outer.Overlapping(inner));
    }

    /// <summary>
    /// Verificerer at to adskilte intervaller
    /// ikke overlapper hinanden.
    /// </summary>
    [Fact]
    public void Overlapping_SeparatedIntervals_ReturnsFalse()
    {
        // Arrange
        var first = CreateInterval(9, 10);
        var second = CreateInterval(11, 12);

        // Act & Assert
        Assert.False(first.Overlapping(second));
    }

    /// <summary>
    /// Verificerer at to intervaller hvor det ene slutter
    /// præcis når det andet starter ikke overlapper.
    /// </summary>
    [Fact]
    public void Overlapping_EndEqualsNextStart_ReturnsFalse()
    {
        // Arrange
        var first = CreateInterval(9, 10);
        var second = CreateInterval(10, 11);

        // Act & Assert
        Assert.False(first.Overlapping(second));
    }

    /// <summary>
    /// Verificerer at et interval overlapper med sig selv.
    /// </summary>
    [Fact]
    public void Overlapping_SameInterval_ReturnsTrue()
    {
        // Arrange
        var first = CreateInterval(9, 10);
        var second = CreateInterval(9, 10);

        // Act & Assert
        Assert.True(first.Overlapping(second));
    }

    // ── Record value equality ─────────────────────────────────────────────────

    /// <summary>
    /// Verificerer at to TimeInterval-instancer med identiske værdier
    /// er value-equal.
    /// </summary>
    [Fact]
    public void Equals_SameValues_ReturnsTrue()
    {
        // Arrange
        var first = CreateInterval(9, 10);
        var second = CreateInterval(9, 10);

        // Act & Assert
        Assert.Equal(first, second);
    }

    /// <summary>
    /// Verificerer at to TimeInterval-instancer med forskellige værdier
    /// ikke er value-equal.
    /// </summary>
    [Fact]
    public void Equals_DifferentValues_ReturnsFalse()
    {
        // Arrange
        var first = CreateInterval(9, 10);
        var second = CreateInterval(11, 12);

        // Act & Assert
        Assert.NotEqual(first, second);
    }
}