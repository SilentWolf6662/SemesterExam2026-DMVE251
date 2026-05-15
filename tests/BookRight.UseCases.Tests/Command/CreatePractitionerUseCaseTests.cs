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

    private static (ICreatePractitionerUseCase UseCase, Mock<IPractitionerRepository> MockRepo) CreateUseCase()
    {
        var mockRepo = new Mock<IPractitionerRepository>();
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Practitioner>())).Returns(Task.CompletedTask);
        mockRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        return (new CreatePractitionerUseCase(mockRepo.Object), mockRepo);
    }

    // ── Persistering ──────────────────────────────────────────────────────────

    // Tester at AddAsync kaldes præcis én gang med en Practitioner-instans.
    [Fact]
    public async Task Execute_WithValidRequest_CallsAddAsync()
    {
        // Arrange
        // Opretter use case med mock repository og et gyldigt request
        var (useCase, mockRepo) = CreateUseCase();
        var request = CreateRequest();

        // Act
        // Udfører oprettelsen af behandleren
        await useCase.Execute(request);

        // Assert
        // Verificerer at behandleren blev sendt til repository præcis én gang
        mockRepo.Verify(r => r.AddAsync(It.IsAny<Practitioner>()), Times.Once);
    }

    // Tester at SaveAsync kaldes præcis én gang efter behandleren er tilføjet.
    [Fact]
    public async Task Execute_WithValidRequest_CallsSaveAsync()
    {
        // Arrange
        var (useCase, mockRepo) = CreateUseCase();
        var request = CreateRequest();

        // Act
        await useCase.Execute(request);

        // Assert
        // Verificerer at ændringerne blev gemt præcis én gang
        mockRepo.Verify(r => r.SaveAsync(), Times.Once);
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
        // Fanger den oprettede behandler for at verificere autorisationstypen
        Practitioner? capturedPractitioner = null;
        var mockRepo = new Mock<IPractitionerRepository>();
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Practitioner>()))
            .Callback<Practitioner>(p => capturedPractitioner = p)
            .Returns(Task.CompletedTask);
        mockRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        ICreatePractitionerUseCase useCase = new CreatePractitionerUseCase(mockRepo.Object);
        var request = CreateRequest(authorization: input);

        // Act
        await useCase.Execute(request);

        // Assert
        // Verificerer at autorisationstypen er parset korrekt
        Assert.NotNull(capturedPractitioner);
        Assert.Equal(expected, capturedPractitioner.Authorization);
    }

    // Tester at en ugyldig autorisationstype kaster en ArgumentException.
    [Fact]
    public async Task Execute_WithInvalidAuthorization_ThrowsArgumentException()
    {
        // Arrange
        // Opretter request med en ugyldig autorisationstype
        var (useCase, _) = CreateUseCase();
        var request = CreateRequest(authorization: "UgyldigType");

        // Act & Assert
        // Verificerer at en ugyldig type resulterer i en undtagelse
        await Assert.ThrowsAnyAsync<ArgumentException>(() => useCase.Execute(request));
    }
}
