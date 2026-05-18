using BookRight.Domain.Entities;
using BookRight.Facade.Command;
using BookRight.Facade.DTO;
using BookRight.Facade.Interfaces.UseCase;
using BookRight.UseCases.Command;
using BookRight.UseCases.Repositories;
using Moq;

namespace BookRight.UseCases.Tests.Command;

// Tests for CreateClinicUseCase.
// Dækker:
//   - At klinik oprettes og persisteres via repository
//   - At åbningstider mappes korrekt fra DTO til domæne-objekt

public class CreateClinicUseCaseTests
{
    // ── Hjælpe-data ───────────────────────────────────────────────────────────

    private static readonly List<TimeIntervalDto> TestWorkingHours =
    [
        new TimeIntervalDto(new DateTime(2026, 6, 1, 8, 0, 0), new DateTime(2026, 6, 1, 17, 0, 0))
    ];

    private static CreateClinicRequest CreateRequest(
        string streetName = "Testvej 1",
        int zipcode = 7100,
        int rooms = 3,
        List<TimeIntervalDto>? workingHours = null) =>
        new CreateClinicRequest(
            streetName,
            zipcode,
            workingHours ?? TestWorkingHours,
            rooms);

    private static (ICreateClinicUseCase UseCase, Mock<IClinicRepository> MockRepo) CreateUseCase()
    {
        var mockRepo = new Mock<IClinicRepository>();
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Clinic>())).Returns(Task.CompletedTask);
        mockRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        return (new CreateClinicUseCase(mockRepo.Object), mockRepo);
    }

    // ── Persistering ──────────────────────────────────────────────────────────

    // Tester at AddAsync og SaveAsync kaldes præcis én gang med en Clinic-instans.
    [Fact]
    public async Task Execute_WithValidRequest_CallsAddAndSaveAsync()
    {
        // Arrange
        // Opretter use case med mock repository og et gyldigt request
        var (useCase, mockRepo) = CreateUseCase();
        var request = CreateRequest();

        // Act
        // Udfører oprettelsen af klinikken
        await useCase.Execute(request);

        // Assert
        // Verificerer at klinikken blev sendt til repository præcis én gang
        mockRepo.Verify(r => r.AddAsync(It.IsAny<Clinic>()), Times.Once);
        // Verificerer at ændringerne blev gemt præcis én gang
        mockRepo.Verify(r => r.SaveAsync(), Times.Once);
    }

    // ── Mapping ───────────────────────────────────────────────────────────────

    // Tester at antallet af åbningstider bevares under mapping fra DTO til domæne-objekt.
    [Fact]
    public async Task Execute_WithMultipleWorkingHours_MapsAllIntervals()
    {
        // Arrange
        // Opretter request med to åbningstidsintervaller
        var workingHours = new List<TimeIntervalDto>
        {
            new TimeIntervalDto(new DateTime(2026, 6, 1, 8, 0, 0), new DateTime(2026, 6, 1, 17, 0, 0)),
            new TimeIntervalDto(new DateTime(2026, 6, 2, 8, 0, 0), new DateTime(2026, 6, 2, 17, 0, 0))
        };

        Clinic? capturedClinic = null;
        var mockRepo = new Mock<IClinicRepository>();
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Clinic>()))
            .Callback<Clinic>(c => capturedClinic = c)
            .Returns(Task.CompletedTask);
        mockRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        ICreateClinicUseCase useCase = new CreateClinicUseCase(mockRepo.Object);
        var request = CreateRequest(workingHours: workingHours);

        // Act
        await useCase.Execute(request);

        // Assert
        // Verificerer at begge intervaller er bevaret i den oprettede klinik
        Assert.NotNull(capturedClinic);
        Assert.Equal(2, capturedClinic.WorkingHours.Count);
    }
}
