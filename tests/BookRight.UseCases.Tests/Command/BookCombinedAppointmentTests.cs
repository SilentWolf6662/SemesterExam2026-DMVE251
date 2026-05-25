using BookRight.Domain.Entities;
using BookRight.Domain.Enums;
using BookRight.Domain.Exceptions;
using BookRight.Domain.ValueObjects;
using BookRight.Facade.Command;
using BookRight.UseCases.Command;
using BookRight.UseCases.Repositories;
using BookRight.UseCases.Services;
using Moq;

namespace BookRight.UseCases.Tests.Command;

// Tests for BookCombinedAppointmentUseCase.
//
// Dækker:
//   - Happy path: to forskellige behandlingstyper oprettes og gemmes
//   - Samme behandlingstype: DomainException smides
//   - Entiteter ikke fundet: NotFoundException smides (patient, behandler, klinik)
//   - Rumsbegrænsning: DomainException hvis klinikken er fuldt booket

public class BookCombinedAppointmentTests
{
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock = new();
    private readonly Mock<IPractitionerRepository> _practitionerRepoMock = new();
    private readonly Mock<IPatientRepository> _patientRepoMock = new();
    private readonly Mock<ITreatmentTypeRepository> _treatmentTypeRepoMock = new();
    private readonly Mock<IClinicRepository> _clinicRepoMock = new();
    private readonly BookCombinedAppointmentUseCase _sut;

    // Faste Guid-værdier der bruges på tværs af hjælpemetoder og tests.
    private readonly Guid _patientId = Guid.NewGuid();
    private readonly Guid _practitionerId1 = Guid.NewGuid();
    private readonly Guid _practitionerId2 = Guid.NewGuid();
    private readonly Guid _clinicId = Guid.NewGuid();
    private readonly Guid _treatment1Id = Guid.NewGuid();
    private readonly Guid _treatment2Id = Guid.NewGuid();

    public BookCombinedAppointmentTests()
    {
        var pricingService = new PricingService(
            [],
            _appointmentRepoMock.Object,
            _patientRepoMock.Object,
            _treatmentTypeRepoMock.Object);

        _sut = new BookCombinedAppointmentUseCase(
            _appointmentRepoMock.Object,
            _practitionerRepoMock.Object,
            _patientRepoMock.Object,
            _treatmentTypeRepoMock.Object,
            _clinicRepoMock.Object,
            pricingService);
    }

    // ── Hjælpemetoder ─────────────────────────────────────────────────────────

    // Bygger en gyldig BookCombinedAppointmentRequest med standardværdier der kan overskrives.
    private BookCombinedAppointmentRequest CreateRequest(
        Guid? patientId = null,
        Guid? clinicId = null,
        Guid? practitionerId1 = null,
        Guid? treatment1Id = null,
        Guid? practitionerId2 = null,
        Guid? treatment2Id = null,
        DateTime? from = null) =>
        new BookCombinedAppointmentRequest(
            from ?? new DateTime(2026, 6, 15, 10, 0, 0),
            patientId ?? _patientId,
            clinicId ?? _clinicId,
            practitionerId1 ?? _practitionerId1,
            treatment1Id ?? _treatment1Id,
            60,
            practitionerId2 ?? _practitionerId2,
            treatment2Id ?? _treatment2Id,
            60);

    // Opsætter alle mocks til en vellykket kombineret booking uden overlap eller kapacitetsproblemer.
    private void SetupValidMocks(int clinicRooms = 5)
    {
        var patient = Patient.Create("Test", "Patient", "12345678", "test@test.dk",
            new DateTime(1985, 3, 14), new Address("Testvej 1", 7100), null, Guid.Empty);
        var practitioner1 = Practitioner.Create("Anna", "Hansen", "11111111", "anna@test.dk",
            AuthorizationType.Physiotherapist, 10001);
        var practitioner2 = Practitioner.Create("Peter", "Jensen", "22222222", "peter@test.dk",
            AuthorizationType.Masseur, 10002);
        var clinic = Clinic.Create(new Address("Klinikgade 1", 7100), [], clinicRooms);
        var type1 = TreatmentType.Create("Fysioterapi", AuthorizationType.Physiotherapist, null,
            [new TreatmentPrice(60, 500m)]);
        var type2 = TreatmentType.Create("Massage", AuthorizationType.Masseur, null,
            [new TreatmentPrice(60, 400m)]);

        _patientRepoMock.Setup(r => r.GetByIdAsync(_patientId)).ReturnsAsync(patient);
        _practitionerRepoMock.Setup(r => r.GetByIdAsync(_practitionerId1)).ReturnsAsync(practitioner1);
        _practitionerRepoMock.Setup(r => r.GetByIdAsync(_practitionerId2)).ReturnsAsync(practitioner2);
        _clinicRepoMock.Setup(r => r.GetByIdAsync(_clinicId)).ReturnsAsync(clinic);
        _treatmentTypeRepoMock.Setup(r => r.GetByIdAsync(_treatment1Id)).ReturnsAsync(type1);
        _treatmentTypeRepoMock.Setup(r => r.GetByIdAsync(_treatment2Id)).ReturnsAsync(type2);

        _appointmentRepoMock.Setup(r => r.GetAllByPatientIdAsync(_patientId)).ReturnsAsync([]);
        _appointmentRepoMock.Setup(r => r.GetAllByPractitionerIdAsync(_practitionerId1)).ReturnsAsync([]);
        _appointmentRepoMock.Setup(r => r.GetAllByPractitionerIdAsync(_practitionerId2)).ReturnsAsync([]);
        _appointmentRepoMock.Setup(r => r.GetAllByClinicIdAsync(_clinicId)).ReturnsAsync([]);
        _appointmentRepoMock.Setup(r => r.GetSumOf12MonthsByPatientIdAsync(_patientId, It.IsAny<DateTime>()))
            .ReturnsAsync(0m);
        _appointmentRepoMock.Setup(r => r.GetBirthdayDiscountUsedCountByPatientIdAsync(_patientId, It.IsAny<DateTime>()))
            .ReturnsAsync(0);
        _appointmentRepoMock.Setup(r => r.AddAsync(It.IsAny<Appointment>())).Returns(Task.CompletedTask);
        _appointmentRepoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
    }

    // ── Happy path ────────────────────────────────────────────────────────────

    // Tester at to appointments oprettes og gemmes præcis én gang ved en gyldig anmodning.
    [Fact]
    public async Task Execute_ValidRequest_AddsBothAppointmentsAndSavesOnce()
    {
        // Arrange
        SetupValidMocks();
        var request = CreateRequest();

        // Act
        await _sut.Execute(request);

        // Assert
        _appointmentRepoMock.Verify(r => r.AddAsync(It.IsAny<Appointment>()), Times.Exactly(2)); // Begge aftaler tilføjes
        _appointmentRepoMock.Verify(r => r.SaveAsync(), Times.Once); // Gemt præcis én gang
    }

    // ── Forretningsregler ─────────────────────────────────────────────────────

    // Tester at DomainException smides når begge behandlingstyper er ens.
    // En kombineret booking kræver to FORSKELLIGE behandlingstyper.
    [Fact]
    public async Task Execute_SameTreatmentType_ThrowsDomainException()
    {
        // Arrange
        SetupValidMocks();
        var request = CreateRequest(treatment2Id: _treatment1Id); // Samme behandlingstype

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _sut.Execute(request));
    }

    // ── NotFoundException ─────────────────────────────────────────────────────

    // Tester at NotFoundException smides når patienten ikke kan findes.
    [Fact]
    public async Task Execute_PatientNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _patientRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Patient?)null);
        var request = CreateRequest();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.Execute(request));
    }

    // Tester at NotFoundException smides når første behandler ikke kan findes.
    [Fact]
    public async Task Execute_FirstPractitionerNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var patient = Patient.Create("Test", "Patient", "12345678", "test@test.dk",
            new DateTime(1985, 3, 14), new Address("Testvej 1", 7100), null, Guid.Empty);
        _patientRepoMock.Setup(r => r.GetByIdAsync(_patientId)).ReturnsAsync(patient);
        _practitionerRepoMock.Setup(r => r.GetByIdAsync(_practitionerId1)).ReturnsAsync((Practitioner?)null);
        var request = CreateRequest();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.Execute(request));
    }

    // Tester at NotFoundException smides når klinikken ikke kan findes.
    [Fact]
    public async Task Execute_ClinicNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var patient = Patient.Create("Test", "Patient", "12345678", "test@test.dk",
            new DateTime(1985, 3, 14), new Address("Testvej 1", 7100), null, Guid.Empty);
        var practitioner1 = Practitioner.Create("Anna", "Hansen", "11111111", "anna@test.dk",
            AuthorizationType.Physiotherapist, 10001);
        var practitioner2 = Practitioner.Create("Peter", "Jensen", "22222222", "peter@test.dk",
            AuthorizationType.Masseur, 10002);

        _patientRepoMock.Setup(r => r.GetByIdAsync(_patientId)).ReturnsAsync(patient);
        _practitionerRepoMock.Setup(r => r.GetByIdAsync(_practitionerId1)).ReturnsAsync(practitioner1);
        _practitionerRepoMock.Setup(r => r.GetByIdAsync(_practitionerId2)).ReturnsAsync(practitioner2);
        _clinicRepoMock.Setup(r => r.GetByIdAsync(_clinicId)).ReturnsAsync((Clinic?)null);
        var request = CreateRequest();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.Execute(request));
    }

    // ── Rumsbegrænsning ───────────────────────────────────────────────────────

    // Opretter et aktivt Appointment der overlapper med tidsintervallet kl. 10–11
    // — bruges til at simulere at et rum er optaget.
    private static Appointment CreateOverlappingActiveAppointment()
    {
        var interval = new TimeInterval(new DateTime(2026, 6, 15, 10, 0, 0), new DateTime(2026, 6, 15, 11, 0, 0));
        return Appointment.Create(interval, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, 100m, [], []);
    }

    // Tester at DomainException smides når alle klinikrummene er optaget i 1. behandlings tidsrum.
    [Fact]
    public async Task Execute_ClinicAtCapacityForFirstSlot_ThrowsDomainException()
    {
        // Arrange
        const int rooms = 1;
        SetupValidMocks(clinicRooms: rooms);

        // Ét aktivt overlap i 1. behandlingens tidsrum (kl. 10–11) → klinik fuldt booket
        var occupyingAppointment = CreateOverlappingActiveAppointment();
        _appointmentRepoMock.Setup(r => r.GetAllByClinicIdAsync(_clinicId))
            .ReturnsAsync([occupyingAppointment]);

        var request = CreateRequest(from: new DateTime(2026, 6, 15, 10, 0, 0));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() => _sut.Execute(request));
        Assert.Contains("1. behandling", ex.Message);
    }

    // Tester at DomainException smides når klinikken er fuldt booket i 2. behandlings tidsrum,
    // men stadig har plads i 1. behandlingens tidsrum.
    [Fact]
    public async Task Execute_ClinicAtCapacityForSecondSlot_ThrowsDomainException()
    {
        // Arrange
        const int rooms = 1;
        SetupValidMocks(clinicRooms: rooms);

        // 2. behandling starter kl. 11 (1. behandling varer 60 min fra kl. 10)
        var secondSlotOccupied = new TimeInterval(new DateTime(2026, 6, 15, 11, 0, 0), new DateTime(2026, 6, 15, 12, 0, 0));
        var occupying = Appointment.Create(secondSlotOccupied, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, 100m, [], []);

        _appointmentRepoMock.Setup(r => r.GetAllByClinicIdAsync(_clinicId))
            .ReturnsAsync([occupying]);

        var request = CreateRequest(from: new DateTime(2026, 6, 15, 10, 0, 0));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() => _sut.Execute(request));
        Assert.Contains("2. behandling", ex.Message);
    }

    // Tester at booking lykkes selv om klinikken har én aktiv booking, forudsat der er nok rum.
    [Fact]
    public async Task Execute_ClinicHasOneBookingButMultipleRooms_Succeeds()
    {
        // Arrange
        SetupValidMocks(clinicRooms: 3); // 3 rum → plads til mere

        var existingAppointment = CreateOverlappingActiveAppointment();
        _appointmentRepoMock.Setup(r => r.GetAllByClinicIdAsync(_clinicId))
            .ReturnsAsync([existingAppointment]); // 1 ud af 3 rum optaget

        var request = CreateRequest(from: new DateTime(2026, 6, 15, 10, 0, 0));

        // Act
        await _sut.Execute(request);

        // Assert
        _appointmentRepoMock.Verify(r => r.AddAsync(It.IsAny<Appointment>()), Times.Exactly(2));
    }
}
