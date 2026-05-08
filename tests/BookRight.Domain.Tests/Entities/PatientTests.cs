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

    // Tester at FirstName matcher det fornavn der gives til Create().
    [Fact]
    public void Create_Patient_HasCorrectFirstName()
    {
    }

    // Tester at LastName matcher det efternavn der gives til Create().
    [Fact]
    public void Create_Patient_HasCorrectLastName()
    {
    }

    // Tester at PhoneNumber matcher det telefonnummer der gives til Create().
    [Fact]
    public void Create_Patient_HasCorrectPhoneNumber()
    {
    }

    // Tester at Email matcher den e-mailadresse der gives til Create().
    [Fact]
    public void Create_Patient_HasCorrectEmail()
    {
    }

    // Tester at Birthday matcher den fødselsdato der gives til Create().
    [Fact]
    public void Create_Patient_HasCorrectBirthday()
    {
    }

    // Tester at Note matcher den tekst der gives til Create().
    [Fact]
    public void Create_Patient_HasCorrectNote()
    {
    }

    // Tester at PreferredPractitioner matcher det behandler-Id der gives til Create().
    [Fact]
    public void Create_Patient_HasCorrectPreferredPractitioner()
    {
    }

    // Tester at to Patient-oprettelser giver to separate objekter i hukommelsen.
    [Fact]
    public void Create_TwoPatients_AreNotSameReference()
    {
    }
}
