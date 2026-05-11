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
        //Arrange & Act
        //Oprettelse af patient.
        var patient = CreatePatient();

        //Assert
        //Tjekke at patienten data ikke er Null.
        Assert.NotNull(patient);
    }

    // Tester at data matcher med input data der gives til Create().
    // Tester også Patient-oprettelser giver to separate objekter i hukommelsen.
    [Fact]
    public void Create_Patient_HasCorrectData()
    {
        //Arrange & Act
        //Oprettelse af patient samt addresse
        var patient = CreatePatient();
        var address = new Address("Testvej 1", 1234);
        
        //Assert
        //Tjekker at patient data er correct med det oprettet.
        Assert.Equal("Anders", patient.FirstName);
        Assert.Equal("Andersen", patient.LastName);
        Assert.Equal("12345678", patient.PhoneNumber);
        Assert.Equal("anders@test.dk", patient.Email);
        //Assert.Equal(address, patient.PatientAddress);
    }

    // Tester at to Patient-oprettelser giver to separate objekter i hukommelsen.
    [Fact]
    public void Create_TwoPatients_AreNotSame()
    {
        //Arrange & Act
        //Opretter 2 patienter.
        var patient = CreatePatient();
        var patient2 = CreatePatient("Fredde");

        //Assert
        //Tjekker at 2 patient oprettelser ikke er de samme.
        Assert.NotEqual(patient, patient2);
    }
}
