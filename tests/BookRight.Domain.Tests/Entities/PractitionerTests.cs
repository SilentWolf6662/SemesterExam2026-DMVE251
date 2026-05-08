using BookRight.Domain.Entities;

namespace BookRight.Domain.Tests.Entities;

/// <summary>
/// Tests for Practitioner-entiteten (aggregat-rod).
///
/// Dækker:
///   - Oprettelse via factory-metoden Create()
///   - At alle personlige felter (fornavn, efternavn, telefon, email) sættes korrekt
///   - At listerne Clinics og Appointments er tomme ved oprettelse
///
///</summary>
public class PractitionerTests
{
    // ── Hjælpe-data ───────────────────────────────────────────────────────────

    /// <summary>
    /// Opretter en Practitioner med standardværdier, der kan overskrives pr. parameter.
    /// </summary>
    private static Practitioner CreatePractitioner(
        string firstName = "Lars",
        string lastName = "Larsen",
        string phoneNumber = "11223344",
        string email = "lars@klinik.dk") =>
        Practitioner.Create(firstName, lastName, phoneNumber, email);

    // ── Oprettelse ────────────────────────────────────────────────────────────

    // Tester at factory-metoden returnerer et Practitioner-objekt og ikke null.
    [Fact]
    public void Create_Practitioner_WithValidValues_ReturnsNotNull()
    {
    }

    // Tester at FirstName matcher det fornavn der gives til Create().
    [Fact]
    public void Create_Practitioner_HasCorrectFirstName()
    {
    }

    // Tester at LastName matcher det efternavn der gives til Create().
    [Fact]
    public void Create_Practitioner_HasCorrectLastName()
    {
    }

    // Tester at PhoneNumber matcher det telefonnummer der gives til Create().
    [Fact]
    public void Create_Practitioner_HasCorrectPhoneNumber()
    {
    }

    // Tester at Email matcher den e-mailadresse der gives til Create().
    [Fact]
    public void Create_Practitioner_HasCorrectEmail()
    {
    }

    // ── Lister ────────────────────────────────────────────────────────────────

    // Tester at en nyoprettet behandler ikke er tilknyttet nogen klinikker.
    [Fact]
    public void Create_Practitioner_HasEmptyClinicsList()
    {
    }

    // Tester at en nyoprettet behandler ikke har nogen bookede aftaler.
    [Fact]
    public void Create_Practitioner_HasEmptyAppointmentsList()
    {
    }

    // Tester at Clinics-listen for to separate behandlere ikke peger på det samme listobjekt.
    [Fact]
    public void Create_TwoPractitioners_DoNotShareClinicsListReference()
    {
    }
}
