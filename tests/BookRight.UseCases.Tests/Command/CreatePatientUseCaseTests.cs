using BookRight.Domain.Entities;
using BookRight.Facade.Command;
using BookRight.Facade.Interfaces.UseCase;
using BookRight.UseCases.Command;
using BookRight.UseCases.Repositories;
using Moq;

namespace BookRight.UseCases.Tests.Command;

// Tests for CreatePatientUseCase.
// Dækker:
//   - At patient oprettes og persisteres via repository
//   - At null foretrukken behandler håndteres korrekt

public class CreatePatientUseCaseTests
{
    private readonly Mock<IPatientRepository> _patientRepoMock;
    private readonly ICreatePatientUseCase _sut; // _sut står for "System Under Test", og er en konvention for at navngive den klasse, som vi tester

    public CreatePatientUseCaseTests()
    {
        _patientRepoMock = new Mock<IPatientRepository>();
        _sut = new CreatePatientUseCase(_patientRepoMock.Object);
    }

    // ── Hjælpe-data ───────────────────────────────────────────────────────────

    private static CreatePatientRequest CreateRequest(
        string firstName = "Anders",
        string lastName = "Andersen",
        string email = "anders@test.dk",
        string phoneNumber = "12345678",
        string streetName = "Testvej 1",
        int zipcode = 7100,
        string note = "",
        Guid? preferredPractitioner = null) =>
        new CreatePatientRequest(
            firstName,
            lastName,
            email,
            phoneNumber,
            new DateTime(1990, 5, 15),
            streetName,
            zipcode,
            note,
            preferredPractitioner);

    // ── Persistering ──────────────────────────────────────────────────────────

    // Tester at AddAsync kaldes præcis én gang med en Patient-instans.
    [Fact]
    public async Task Execute_WithValidRequest_CallsAddAsync()
    {
        // Arrange
        var request = CreateRequest();
        _patientRepoMock.Setup(r => r.AddAsync(It.IsAny<Patient>())).Returns(Task.CompletedTask); // Simulerer, at tilføjelsen af en patient lykkes

        // Act
        await _sut.Execute(request);

        // Assert
        _patientRepoMock.Verify(r => r.AddAsync(It.IsAny<Patient>()), Times.Once); // Verificerer at patienten blev sendt til repository præcis én gang
    }

    // Tester at SaveAsync kaldes præcis én gang efter patienten er tilføjet.
    [Fact]
    public async Task Execute_WithValidRequest_CallsSaveAsync()
    {
        // Arrange
        var request = CreateRequest();

        // Act
        await _sut.Execute(request);

        // Assert
        _patientRepoMock.Verify(r => r.SaveAsync(), Times.Once); // Verificerer at ændringerne blev gemt præcis én gang
    }

    // ── Foretrukken behandler ─────────────────────────────────────────────────

    // Tester at null foretrukken behandler resulterer i Guid.Empty på den oprettede patient.
    [Fact]
    public async Task Execute_WithNullPreferredPractitioner_UsesEmptyGuid()
    {
        // Arrange
        Patient? capturedPatient = null;
        _patientRepoMock.Setup(r => r.AddAsync(It.IsAny<Patient>()))
            .Callback<Patient>(p => capturedPatient = p)
            .Returns(Task.CompletedTask);

        var request = CreateRequest(preferredPractitioner: null);

        // Act
        await _sut.Execute(request);

        // Assert
        Assert.NotNull(capturedPatient);
        Assert.Equal(Guid.Empty, capturedPatient.PreferredPractitioner); // Verificerer at Guid.Empty bruges som foretrukken behandler når ingen er angivet
    }
}
