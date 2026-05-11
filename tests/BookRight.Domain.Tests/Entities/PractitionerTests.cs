using BookRight.Domain.Entities;
using BookRight.Domain.ValueObjects;
using System.Net;

namespace BookRight.Domain.Tests.Entities;

// Tests for Practitioner-entiteten (aggregat-rod).

// Dækker:
//   - Oprettelse via factory-metoden Create()
//   - At alle personlige felter (fornavn, efternavn, telefon, email) sættes korrekt
//   - At listerne Clinics og Appointments er tomme ved oprettelse
public class PractitionerTests
{
    // ── Hjælpe-data ───────────────────────────────────────────────────────────

    // Opretter en Practitioner med standardværdier, der kan overskrives pr. parameter.
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
        // Arrange & Act
        var practitioner = CreatePractitioner();

        Assert.NotNull(practitioner);
    }

    // Tester at data matcher det input data der gives til Create().
    [Fact]
    public void Create_Practitioner_HasCorrectData()
    {
        // Arrange & Act
        var practitioner = CreatePractitioner();

        // Assert
        Assert.Equal("Lars", practitioner.FirstName);
        Assert.Equal("Larsen", practitioner.LastName);
        Assert.Equal("11223344", practitioner.PhoneNumber);
        Assert.Equal("lars@klinik.dk", practitioner.Email);
    }

    // ── Lister ────────────────────────────────────────────────────────────────

    // Tester at en nyoprettet behandler ikke er tilknyttet nogen klinikker eller har nogen bookede aftaler.
    [Fact]
    public void Create_Practitioner_HasEmptyClinicsAndAppointmentsList()
    {
        // Arrange & Act
        var practitioner = CreatePractitioner();

        // Assert
        Assert.Empty(practitioner.Clinics);
        Assert.Empty(practitioner.Appointments);
    }
}
