using BookRight.Domain.Entities;
using BookRight.Domain.ValueObjects;

namespace BookRight.Domain.Tests.Entities;

/// <summary>
/// Tests for Clinic-entiteten (aggregat-rod).
///
/// Dækker:
///   - Oprettelse via factory-metoden Create()
///   - At adresse, åbningstider og antal rum sættes korrekt
///
///</summary>
public class ClinicTests
{
    // ── Hjælpe-data ───────────────────────────────────────────────────────────

    private static readonly Address TestAddress = new Address("Klinikgade 5", 8000);

    /// <summary>
    /// Opretter en simpel liste med ét åbningstids-interval (08:00-16:00).
    /// </summary>
    private static List<TimeInterval> CreateOpeningHours() =>
    [
        new TimeInterval(
            new DateTime(2026, 1, 1, 8, 0, 0),
            new DateTime(2026, 1, 1, 16, 0, 0))
    ];

    /// <summary>
    /// Opretter en Clinic med standardværdier der kan overskrives pr. parameter.
    /// </summary>
    private static Clinic CreateClinic(int rooms = 3, List<TimeInterval>? openingHours = null) =>
        Clinic.Create(TestAddress, openingHours ?? CreateOpeningHours(), rooms);

    // ── Oprettelse ────────────────────────────────────────────────────────────

    // Tester at factory-metoden returnerer et Clinic-objekt og ikke null.
    [Fact]
    public void Create_Clinic_WithValidValues_ReturnsNotNull()
    {
    }

    // Tester at Rooms-feltet matcher det antal rum der gives til Create().
    [Fact]
    public void Create_Clinic_HasCorrectNumberOfRooms()
    {

    }

    // Tester grænseværdien: en klinik med præcis ét rum er gyldig.
    [Fact]
    public void Create_Clinic_WithOneRoom_HasOneRoom()
    {
    }

    // Tester at ClinicAddress peger på den adresse der gives til Create().
    [Fact]
    public void Create_Clinic_HasCorrectAddress()
    {
    }

    // Tester at WorkingHours indeholder præcist det antal tidsintervaller der gives til Create().
    [Fact]
    public void Create_Clinic_HasCorrectNumberOfOpeningHours()
    {
    }

    // Tester at klinikken kan gemme flere åbningstids-intervaller (fx formiddag og eftermiddag).
    [Fact]
    public void Create_Clinic_MultipleOpeningHours_StoredAsList()
    {
    }

    // Tester at det første åbningstids-interval i WorkingHours stemmer overens med input.
    [Fact]
    public void Create_Clinic_OpeningHoursMatchInputIntervals()
    {
    }

    // Tester at WorkingHours-listen for to separate klinikker ikke peger på det samme listobjekt.
    [Fact]
    public void Create_TwoClinics_DoNotShareOpeningHoursListReference()
    {
    }
}
