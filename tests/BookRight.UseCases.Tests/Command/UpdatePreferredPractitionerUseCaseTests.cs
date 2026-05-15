using BookRight.Domain.Entities;
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
            Domain.Enums.AuthorizationType.Physiotherapist, 10001);

    private static (IUpdatePreferredPractitionerUseCase UseCase, Mock<IPatientRepository> PatientMock, Mock<IPractitionerRepository> PractitionerMock)
        CreateUseCase(Patient patient, Practitioner? practitioner = null)
    {
        var patientMock = new Mock<IPatientRepository>();
        patientMock.Setup(r => r.GetByIdAsync(patient.Id)).ReturnsAsync(patient);
        patientMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var practitionerMock = new Mock<IPractitionerRepository>();
        if (practitioner is not null)
            practitionerMock.Setup(r => r.GetByIdAsync(practitioner.Id)).ReturnsAsync(practitioner);

        IUpdatePreferredPractitionerUseCase useCase =
            new UpdatePreferredPractitionerUseCase(patientMock.Object, practitionerMock.Object);

        return (useCase, patientMock, practitionerMock);
    }

    // ── Opdatering ────────────────────────────────────────────────────────────

    // Tester at foretrukken behandler opdateres til den nye værdi på patienten.
    [Fact]
    public async Task Execute_WithExistingPatientAndPractitioner_UpdatesPreferredPractitioner()
    {
        // Arrange
        // Opretter en patient og en behandler der begge findes i systemet
        var patient = CreatePatient();
        var newPractitioner = CreatePractitioner();
        var (useCase, _, _) = CreateUseCase(patient, newPractitioner);
        var request = new UpdatePreferredPractitionerRequest(patient.Id, newPractitioner.Id);

        // Act
        // Udfører opdateringen af foretrukken behandler
        await useCase.Execute(request);

        // Assert
        // Verificerer at den foretrukne behandler er opdateret til den nye behandlers Id
        Assert.Equal(newPractitioner.Id, patient.PreferredPractitioner);
    }

    // Tester at SaveAsync kaldes præcis én gang efter opdateringen.
    [Fact]
    public async Task Execute_WithExistingPatientAndPractitioner_CallsSaveAsync()
    {
        // Arrange
        var patient = CreatePatient();
        var newPractitioner = CreatePractitioner();
        var (useCase, patientMock, _) = CreateUseCase(patient, newPractitioner);
        var request = new UpdatePreferredPractitionerRequest(patient.Id, newPractitioner.Id);

        // Act
        await useCase.Execute(request);

        // Assert
        // Verificerer at ændringerne blev gemt præcis én gang
        patientMock.Verify(r => r.SaveAsync(), Times.Once);
    }

    // ── Fejlhåndtering ────────────────────────────────────────────────────────

    // Tester at et ukendt patient-id kaster en NotFoundException.
    [Fact]
    public async Task Execute_WithUnknownPatientId_ThrowsNotFoundException()
    {
        // Arrange
        // Opsætter repository til at returnere null for et ukendt patient-id
        var unknownPatientId = Guid.NewGuid();
        var practitioner = CreatePractitioner();

        var patientMock = new Mock<IPatientRepository>();
        patientMock.Setup(r => r.GetByIdAsync(unknownPatientId)).ReturnsAsync((Patient?)null);

        var practitionerMock = new Mock<IPractitionerRepository>();
        practitionerMock.Setup(r => r.GetByIdAsync(practitioner.Id)).ReturnsAsync(practitioner);

        IUpdatePreferredPractitionerUseCase useCase =
            new UpdatePreferredPractitionerUseCase(patientMock.Object, practitionerMock.Object);
        var request = new UpdatePreferredPractitionerRequest(unknownPatientId, practitioner.Id);

        // Act & Assert
        // Verificerer at en ukendt patient resulterer i en NotFoundException
        await Assert.ThrowsAsync<NotFoundException>(() => useCase.Execute(request));
    }

    // Tester at et ukendt behandler-id kaster en NotFoundException.
    [Fact]
    public async Task Execute_WithUnknownPractitionerId_ThrowsNotFoundException()
    {
        // Arrange
        // Opsætter repository til at returnere null for et ukendt behandler-id
        var patient = CreatePatient();
        var unknownPractitionerId = Guid.NewGuid();

        var patientMock = new Mock<IPatientRepository>();
        patientMock.Setup(r => r.GetByIdAsync(patient.Id)).ReturnsAsync(patient);

        var practitionerMock = new Mock<IPractitionerRepository>();
        practitionerMock.Setup(r => r.GetByIdAsync(unknownPractitionerId)).ReturnsAsync((Practitioner?)null);

        IUpdatePreferredPractitionerUseCase useCase =
            new UpdatePreferredPractitionerUseCase(patientMock.Object, practitionerMock.Object);
        var request = new UpdatePreferredPractitionerRequest(patient.Id, unknownPractitionerId);

        // Act & Assert
        // Verificerer at en ukendt behandler resulterer i en NotFoundException
        await Assert.ThrowsAsync<NotFoundException>(() => useCase.Execute(request));
    }
}
