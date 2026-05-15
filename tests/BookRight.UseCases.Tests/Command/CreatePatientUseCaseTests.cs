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

    private static (ICreatePatientUseCase UseCase, Mock<IPatientRepository> MockRepo) CreateUseCase()
    {
        var mockRepo = new Mock<IPatientRepository>();
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Patient>())).Returns(Task.CompletedTask);
        mockRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        return (new CreatePatientUseCase(mockRepo.Object), mockRepo);
    }

    // ── Persistering ──────────────────────────────────────────────────────────

    // Tester at AddAsync kaldes præcis én gang med en Patient-instans.
    [Fact]
    public async Task Execute_WithValidRequest_CallsAddAsync()
    {
        // Arrange
        // Opretter use case med mock repository og et gyldigt request
        var (useCase, mockRepo) = CreateUseCase();
        var request = CreateRequest();

        // Act
        // Udfører oprettelsen af patienten
        await useCase.Execute(request);

        // Assert
        // Verificerer at patienten blev sendt til repository præcis én gang
        mockRepo.Verify(r => r.AddAsync(It.IsAny<Patient>()), Times.Once);
    }

    // Tester at SaveAsync kaldes præcis én gang efter patienten er tilføjet.
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

    // ── Foretrukken behandler ─────────────────────────────────────────────────

    // Tester at null foretrukken behandler resulterer i Guid.Empty på den oprettede patient.
    [Fact]
    public async Task Execute_WithNullPreferredPractitioner_UsesEmptyGuid()
    {
        // Arrange
        // Opretter request uden foretrukken behandler
        Patient? capturedPatient = null;
        var mockRepo = new Mock<IPatientRepository>();
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Patient>()))
            .Callback<Patient>(p => capturedPatient = p)
            .Returns(Task.CompletedTask);
        mockRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        ICreatePatientUseCase useCase = new CreatePatientUseCase(mockRepo.Object);
        var request = CreateRequest(preferredPractitioner: null);

        // Act
        await useCase.Execute(request);

        // Assert
        // Verificerer at Guid.Empty bruges som foretrukken behandler når ingen er angivet
        Assert.NotNull(capturedPatient);
        Assert.Equal(Guid.Empty, capturedPatient.PreferredPractitioner);
    }
}
