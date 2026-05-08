using BookRight.Domain.ValueObjects;

namespace BookRight.Domain.Tests.ValueObjects;

/// <summary>
/// Tests for TimeInterval-value-objektet.
///
/// Dækker:
///   - Konstruktør-validering (ugyldig rækkefølge kaster DomainException)
///   - Varighed-beregning (Varighed-egenskaben)
///   - Overlap-logik (Overlapping-metoden) inkl. kanttilfælde
///   - Record value-equality
///
///</summary>
public class TimeIntervalTests
{
    // ── Hjælpe-data ───────────────────────────────────────────────────────────

    /// <summary>
    /// Opretter et TimeInterval på en fast testdato (2026-01-01) mellem de angivne timer.
    /// </summary>
    private static TimeInterval CreateInterval(int startTime, int endTime) =>
        new TimeInterval(
            new DateTime(2026, 1, 1, startTime, 0, 0),
            new DateTime(2026, 1, 1, endTime, 0, 0));

    // ── Konstruktør-validering ────────────────────────────────────────────────

    // Tester at Start og End sættes korrekt når et gyldigt tidsinterval oprettes.
    [Fact]
    public void Create_TimeInterval_WithValidTimes_SetsStartAndEndCorrectly()
    {
    }

    // Tester at konstruktøren kaster DomainException når sluttiden er før starttiden.
    [Fact]
    public void Create_TimeInterval_WithEndBeforeStart_ThrowsDomainException()
    {
    }

    // Tester at konstruktøren kaster DomainException når start og slut er identiske (varighed nul).
    [Fact]
    public void Create_TimeInterval_WithEndEqualToStart_ThrowsDomainException()
    {
    }

    // Tester grænseværdien: et interval med præcis ét minuts varighed er gyldigt.
    [Fact]
    public void Create_TimeInterval_WithOneMinuteDuration_IsValid()
    {
    }

    // ── Varighed ──────────────────────────────────────────────────────────────

    // Tester at Varighed returnerer den korrekte TimeSpan for et times interval.
    [Fact]
    public void Duration_OneHourInterval_ReturnsOneHour()
    {
    }

    // Tester at Varighed returnerer den korrekte TimeSpan for et toTimers interval.
    [Fact]
    public void Duration_TwoHourInterval_ReturnsTwoHours()
    {
    }

    // ── Overlapping ───────────────────────────────────────────────────────────

    // Tester at Overlapping returnerer true når to intervaller delvist overlapper.
    [Fact]
    public void Overlapping_PartialOverlap_ReturnsTrue()
    {
    }

    // Tester at Overlapping er symmetrisk: a.Overlapping(b) giver samme resultat som b.Overlapping(a).
    [Fact]
    public void Overlapping_IsSymmetric()
    {
    }

    // Tester at Overlapping returnerer true når et interval er fuldstændig indeholdt i et andet.
    [Fact]
    public void Overlapping_OneIntervalFullyInsideAnother_ReturnsTrue()
    {
    }

    // Tester at Overlapping returnerer false når to intervaller ikke berører hinanden.
    [Fact]
    public void Overlapping_NoOverlap_ReturnsFalse()
    {
    }

    // Tester kanttilfældet: slut lig næstes start er IKKE et overlap (grænsen er eksklusiv).
    [Fact]
    public void Overlapping_EndEqualToOtherStart_ReturnsFalse()
    {
    }

    // Tester at et interval overlapper med sig selv.
    [Fact]
    public void Overlapping_SameInterval_ReturnsTrue()
    {
    }

    // ── Record value-equality ─────────────────────────────────────────────────

    // Tester at to TimeInterval-instanser med identiske Start og End er value-equal (record-semantik).
    [Fact]
    public void TimeInterval_WithSameValues_AreEqual()
    {
    }

    // Tester at to TimeInterval-instanser med forskellige værdier ikke er ens.
    [Fact]
    public void TimeInterval_WithDifferentValues_AreNotEqual()
    {
    }
}
