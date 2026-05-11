using BookRight.Domain.Entities;
using BookRight.Domain.ValueObjects;

namespace BookRight.Domain.Tests.Entities;

/// <summary>
/// Tests for Patient-entiteten (aggregat-rod).
///
/// Dækker:
///   - Oprettelse via factory-metoden Create()
///   - At alle felter (navn, telefon, email, fødselsdato, note, foretrukken behandler) sættes korrekt
///
///</summary>
public class PatientTests
{
    // ── Hjælpe-data ───────────────────────────────────────────────────────────

    private static readonly Address TestAddress = new Address("Testvej 1", 1234);

    private static readonly Guid PreferredPractitionerId = Guid.NewGuid();

    /// <summary>
    /// Opretter en Patient med standardværdier, der kan overskrives pr. parameter.
    /// </summary>
    private static Patient CreatePatient(
        string firstName = "Anders",
        string lastName = "Andersen",
        string phoneNumber = "12345678",
        string email = "anders@test.dk",
        DateTime? birthday = null,
        string note = "Ingen noter",
        Guid? practitionerId = null) =>
        Patient.Create(
            firstName,
            lastName,
            phoneNumber,
            email,
            birthday ?? new DateTime(1990, 5, 15),
            TestAddress,
            note,
            practitionerId ?? PreferredPractitionerId);

    // ── Oprettelse ────────────────────────────────────────────────────────────

    // Tester at factory-metoden returnerer et Patient-objekt og ikke null.
    [Fact]
    public void Create_Patient_WithValidValues_ReturnsNotNull()
    {
    }

    // Tester at data matcher med input data der gives til Create().
    // Tester også Patient-oprettelser giver to separate objekter i hukommelsen.
    [Fact]
    public void Create_Patient_HasCorrectData()
    {
    }

    // Tester at to Patient-oprettelser giver to separate objekter i hukommelsen.
    [Fact]
    public void Create_TwoPatients_AreNotSameReference()
    {
    }
}
