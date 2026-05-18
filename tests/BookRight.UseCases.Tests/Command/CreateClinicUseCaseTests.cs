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
    private readonly Mock<IClinicRepository> _clinicRepoMock;
    private readonly ICreateClinicUseCase _sut; // _sut står for "System Under Test", og er en konvention for at navngive den klasse, som vi tester

    public CreateClinicUseCaseTests()
    {
        _clinicRepoMock = new Mock<IClinicRepository>();
        _sut = new CreateClinicUseCase(_clinicRepoMock.Object);
    }

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

    // ── Persistering ──────────────────────────────────────────────────────────

    // Tester at AddAsync og SaveAsync kaldes præcis én gang med en Clinic-instans.
    [Fact]
    public async Task Execute_WithValidRequest_CallsAddAndSaveAsync()
    {
        // Arrange
        var request = CreateRequest();
        _clinicRepoMock.Setup(r => r.AddAsync(It.IsAny<Clinic>())).Returns(Task.CompletedTask); // Simulerer, at tilføjelsen af en klinik lykkes

        // Act
        await _sut.Execute(request);

        // Assert
        _clinicRepoMock.Verify(r => r.AddAsync(It.IsAny<Clinic>()), Times.Once); // Verificerer at klinikken blev sendt til repository præcis én gang
        _clinicRepoMock.Verify(r => r.SaveAsync(), Times.Once); // Verificerer at ændringerne blev gemt præcis én gang
    }

    // ── Mapping ───────────────────────────────────────────────────────────────

    // Tester at antallet af åbningstider bevares under mapping fra DTO til domæne-objekt.
    [Fact]
    public async Task Execute_WithMultipleWorkingHours_MapsAllIntervals()
    {
        // Arrange
        var workingHours = new List<TimeIntervalDto>
        {
            new TimeIntervalDto(new DateTime(2026, 6, 1, 8, 0, 0), new DateTime(2026, 6, 1, 17, 0, 0)),
            new TimeIntervalDto(new DateTime(2026, 6, 2, 8, 0, 0), new DateTime(2026, 6, 2, 17, 0, 0))
        };

        Clinic? capturedClinic = null;
        _clinicRepoMock.Setup(r => r.AddAsync(It.IsAny<Clinic>()))
            .Callback<Clinic>(c => capturedClinic = c)
            .Returns(Task.CompletedTask);

        var request = CreateRequest(workingHours: workingHours);

        // Act
        await _sut.Execute(request);

        // Assert
        Assert.NotNull(capturedClinic);
        Assert.Equal(2, capturedClinic.WorkingHours.Count); // Verificerer at begge intervaller er bevaret i den oprettede klinik
    }
}
