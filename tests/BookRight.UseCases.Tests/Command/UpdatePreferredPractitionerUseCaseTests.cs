using BookRight.Domain.Entities;
using BookRight.Domain.Enums;
using BookRight.Domain.Exceptions;
using BookRight.Domain.ValueObjects;
using BookRight.Facade.Command;
using BookRight.Facade.Interfaces.UseCase;
using BookRight.UseCases.Command;
using BookRight.UseCases.Repositories;
using Moq;

namespace BookRight.UseCases.Tests.Command;

// Tests for UpdatePreferredPractitionerUseCase.
// Dækker:
//   - At foretrukken behandler opdateres korrekt på patienten
//   - At ændringerne gemmes via repository
//   - At en ukendt patient-id kaster en NotFoundException
//   - At en ukendt behandler-id kaster en NotFoundException

public class UpdatePreferredPractitionerUseCaseTests
{
    private readonly Mock<IPatientRepository> _patientRepoMock;
    private readonly Mock<IPractitionerRepository> _practitionerRepoMock;
    private readonly IUpdatePreferredPractitionerUseCase _sut; // _sut står for "System Under Test", og er en konvention for at navngive den klasse, som vi tester

    public UpdatePreferredPractitionerUseCaseTests()
    {
        _patientRepoMock = new Mock<IPatientRepository>();
        _practitionerRepoMock = new Mock<IPractitionerRepository>();
        _sut = new UpdatePreferredPractitionerUseCase(_patientRepoMock.Object, _practitionerRepoMock.Object);
    }

    // ── Hjælpe-data ───────────────────────────────────────────────────────────

    private static readonly Address TestAddress = new Address("Testvej 1", 7100);

    private static Patient CreatePatient(Guid? preferredPractitionerId = null) =>
        Patient.Create(
            "Anders", "Andersen", "12345678", "anders@test.dk",
            new DateTime(1990, 5, 15),
            TestAddress,
            "",
            preferredPractitionerId ?? Guid.NewGuid());

    private static Practitioner CreatePractitioner() =>
        Practitioner.Create("Anna", "Hansen", "12345678", "anna@test.dk",
            AuthorizationType.Physiotherapist, 10001);

    // ── Opdatering ────────────────────────────────────────────────────────────

    // Tester at foretrukken behandler opdateres til den nye værdi på patienten.
    [Fact]
    public async Task Execute_WithExistingPatientAndPractitioner_UpdatesPreferredPractitioner()
    {
        // Arrange
        var patient = CreatePatient();
        var newPractitioner = CreatePractitioner();
        _patientRepoMock.Setup(r => r.GetByIdAsync(patient.Id)).ReturnsAsync(patient); // Returnerer en eksisterende patient
        _patientRepoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        _practitionerRepoMock.Setup(r => r.GetByIdAsync(newPractitioner.Id)).ReturnsAsync(newPractitioner); // Returnerer en eksisterende behandler
        var request = new UpdatePreferredPractitionerRequest(patient.Id, newPractitioner.Id);

        // Act
        await _sut.Execute(request);

        // Assert
        Assert.Equal(newPractitioner.Id, patient.PreferredPractitioner); // Verificerer at den foretrukne behandler er opdateret til den nye behandlers Id
    }

    // Tester at SaveAsync kaldes præcis én gang efter opdateringen.
    [Fact]
    public async Task Execute_WithExistingPatientAndPractitioner_CallsSaveAsync()
    {
        // Arrange
        var patient = CreatePatient();
        var newPractitioner = CreatePractitioner();
        _patientRepoMock.Setup(r => r.GetByIdAsync(patient.Id)).ReturnsAsync(patient); // Returnerer en eksisterende patient
        _patientRepoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        _practitionerRepoMock.Setup(r => r.GetByIdAsync(newPractitioner.Id)).ReturnsAsync(newPractitioner); // Returnerer en eksisterende behandler
        var request = new UpdatePreferredPractitionerRequest(patient.Id, newPractitioner.Id);

        // Act
        await _sut.Execute(request);

        // Assert
        _patientRepoMock.Verify(r => r.SaveAsync(), Times.Once); // Verificerer at ændringerne blev gemt præcis én gang
    }

    // ── Fejlhåndtering ────────────────────────────────────────────────────────

    // Tester at et ukendt patient-id kaster en NotFoundException.
    [Fact]
    public async Task Execute_WithUnknownPatientId_ThrowsNotFoundException()
    {
        // Arrange
        var unknownPatientId = Guid.NewGuid();
        var practitioner = CreatePractitioner();
        _patientRepoMock.Setup(r => r.GetByIdAsync(unknownPatientId)).ReturnsAsync((Patient?)null); // Simulerer at patienten ikke findes
        _practitionerRepoMock.Setup(r => r.GetByIdAsync(practitioner.Id)).ReturnsAsync(practitioner);
        var request = new UpdatePreferredPractitionerRequest(unknownPatientId, practitioner.Id);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.Execute(request)); // Verificerer at en ukendt patient resulterer i en NotFoundException
    }

    // Tester at et ukendt behandler-id kaster en NotFoundException.
    [Fact]
    public async Task Execute_WithUnknownPractitionerId_ThrowsNotFoundException()
    {
        // Arrange
        var patient = CreatePatient();
        var unknownPractitionerId = Guid.NewGuid();
        _patientRepoMock.Setup(r => r.GetByIdAsync(patient.Id)).ReturnsAsync(patient); // Returnerer en eksisterende patient
        _practitionerRepoMock.Setup(r => r.GetByIdAsync(unknownPractitionerId)).ReturnsAsync((Practitioner?)null); // Simulerer at behandleren ikke findes
        var request = new UpdatePreferredPractitionerRequest(patient.Id, unknownPractitionerId);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.Execute(request)); // Verificerer at en ukendt behandler resulterer i en NotFoundException
    }
}
