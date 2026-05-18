using BookRight.Domain.Entities;
using BookRight.Domain.Enums;
using BookRight.Facade.Command;
using BookRight.Facade.Interfaces.UseCase;
using BookRight.UseCases.Command;
using BookRight.UseCases.Repositories;
using Moq;

namespace BookRight.UseCases.Tests.Command;

// Tests for CreatePractitionerUseCase.
// Dækker:
//   - At behandler oprettes og persisteres via repository
//   - At autorisationstype parses korrekt fra streng
//   - At ugyldig autorisationstype kaster en fejl

public class CreatePractitionerUseCaseTests
{
    private readonly Mock<IPractitionerRepository> _practitionerRepoMock;
    private readonly ICreatePractitionerUseCase _sut; // _sut står for "System Under Test", og er en konvention for at navngive den klasse, som vi tester

    public CreatePractitionerUseCaseTests()
    {
        _practitionerRepoMock = new Mock<IPractitionerRepository>();
        _sut = new CreatePractitionerUseCase(_practitionerRepoMock.Object);
    }

    // ── Hjælpe-data ───────────────────────────────────────────────────────────

    private static CreatePractitionerRequest CreateRequest(
        string firstName = "Anna",
        string lastName = "Hansen",
        string phoneNumber = "12345678",
        string email = "anna@test.dk",
        string authorization = "Physiotherapist",
        int authorizationNumber = 10001) =>
        new CreatePractitionerRequest(
            firstName,
            lastName,
            phoneNumber,
            email,
            authorization,
            authorizationNumber);

    // ── Persistering ──────────────────────────────────────────────────────────

    // Tester at AddAsync kaldes præcis én gang med en Practitioner-instans.
    [Fact]
    public async Task Execute_WithValidRequest_CallsAddAsync()
    {
        // Arrange
        var request = CreateRequest();
        _practitionerRepoMock.Setup(r => r.AddAsync(It.IsAny<Practitioner>())).Returns(Task.CompletedTask); // Simulerer, at tilføjelsen af en behandler lykkes

        // Act
        await _sut.Execute(request);

        // Assert
        _practitionerRepoMock.Verify(r => r.AddAsync(It.IsAny<Practitioner>()), Times.Once); // Verificerer at behandleren blev sendt til repository præcis én gang
    }

    // Tester at SaveAsync kaldes præcis én gang efter behandleren er tilføjet.
    [Fact]
    public async Task Execute_WithValidRequest_CallsSaveAsync()
    {
        // Arrange
        var request = CreateRequest();

        // Act
        await _sut.Execute(request);

        // Assert
        _practitionerRepoMock.Verify(r => r.SaveAsync(), Times.Once); // Verificerer at ændringerne blev gemt præcis én gang
    }

    // ── Autorisationstype ─────────────────────────────────────────────────────

    // Tester at alle gyldige autorisationstyper parses korrekt fra streng.
    [Theory]
    [InlineData("Physiotherapist", AuthorizationType.Physiotherapist)]
    [InlineData("Masseur",         AuthorizationType.Masseur)]
    [InlineData("Acupuncturist",   AuthorizationType.Acupuncturist)]
    [InlineData("Dietician",       AuthorizationType.Dietician)]
    public async Task Execute_WithValidAuthorization_ParsesCorrectly(string input, AuthorizationType expected)
    {
        // Arrange
        Practitioner? capturedPractitioner = null;
        _practitionerRepoMock.Setup(r => r.AddAsync(It.IsAny<Practitioner>()))
            .Callback<Practitioner>(p => capturedPractitioner = p)
            .Returns(Task.CompletedTask);

        var request = CreateRequest(authorization: input);

        // Act
        await _sut.Execute(request);

        // Assert
        Assert.NotNull(capturedPractitioner);
        Assert.Equal(expected, capturedPractitioner.Authorization); // Verificerer at autorisationstypen er parset korrekt
    }

    // Tester at en ugyldig autorisationstype kaster en ArgumentException.
    [Fact]
    public async Task Execute_WithInvalidAuthorization_ThrowsArgumentException()
    {
        // Arrange
        var request = CreateRequest(authorization: "UgyldigType");

        // Act & Assert
        await Assert.ThrowsAnyAsync<ArgumentException>(() => _sut.Execute(request)); // Verificerer at en ugyldig type resulterer i en undtagelse
    }
}
