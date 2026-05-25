using BookRight.Domain.Entities;
using BookRight.Domain.Enums;
using BookRight.Domain.ValueObjects;
using BookRight.Facade.Command;
using BookRight.UseCases.Command;
using BookRight.UseCases.Repositories;
using BookRight.UseCases.Services;
using Moq;

namespace BookRight.UseCases.Tests.Command;

public class BookAppointmentTests
{
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
    private readonly Mock<IPractitionerRepository> _practitionerRepoMock;
    private readonly Mock<IPatientRepository> _patientRepoMock;
    private readonly Mock<ITreatmentTypeRepository> _treatmentTypeRepoMock;
    private readonly Mock<IClinicRepository> _clinicRepoMock;
    private readonly BookAppointmentUseCase _sut; // _sut står for "System Under Test", og er en konvention for at navngive den klasse, som vi tester

    public BookAppointmentTests()
    {
        _appointmentRepoMock = new Mock<IAppointmentRepository>();
        _practitionerRepoMock = new Mock<IPractitionerRepository>();
        _patientRepoMock = new Mock<IPatientRepository>();
        _treatmentTypeRepoMock = new Mock<ITreatmentTypeRepository>();
        _clinicRepoMock = new Mock<IClinicRepository>();
        var pricingService = new PricingService([], _appointmentRepoMock.Object, _patientRepoMock.Object, _treatmentTypeRepoMock.Object);
        _sut = new BookAppointmentUseCase(_appointmentRepoMock.Object, _practitionerRepoMock.Object, _patientRepoMock.Object, _treatmentTypeRepoMock.Object, pricingService, _clinicRepoMock.Object);
    }

    [Fact]
    public async Task Execute_ValidRequest_AddsAppointmentSuccessfully()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var practitionerId = Guid.NewGuid();
        var treatmentTypeId = Guid.NewGuid();
        var clinicId = Guid.NewGuid();
        var start = DateTime.UtcNow.AddDays(1); // Brug tidspunktet nu og tilføj en dag, så det er i fremtiden
        var end = start.AddHours(1); // Lig 1 time på start, så end er senere
        const int durationMinutes = 60;

        var request = new BookAppointmentRequest(start, end, durationMinutes, treatmentTypeId, patientId, practitionerId, clinicId);

        var mockPatient = Patient.Create("Test", "Patient", "12345678", "test@test.dk", DateTime.UtcNow.AddYears(-30), new Address("Testvej 1", 7100), "", Guid.Empty);
        var mockPractitioner = Practitioner.Create("Test", "Practitioner", "87654321", "practitioner@test.dk", AuthorizationType.Physiotherapist, 12345);
        var mockTreatmentType = TreatmentType.Create("Test behandling", AuthorizationType.Physiotherapist, null, [new TreatmentPrice(durationMinutes, 500m)]);
        var mockClinic = Clinic.Create(new Address("Klinikgade 1", 7100), [], 5);
        var existingPatientAppointments = new List<Appointment>();
        var existingPractitionerAppointments = new List<Appointment>();

        _patientRepoMock.Setup(x => x.GetByIdAsync(patientId)).ReturnsAsync(mockPatient);
        _practitionerRepoMock.Setup(x => x.GetByIdAsync(practitionerId)).ReturnsAsync(mockPractitioner);
        _treatmentTypeRepoMock.Setup(x => x.GetByIdAsync(treatmentTypeId)).ReturnsAsync(mockTreatmentType);
        _clinicRepoMock.Setup(x => x.GetByIdAsync(clinicId)).ReturnsAsync(mockClinic);

        _appointmentRepoMock.Setup(x => x.GetAllByPatientIdAsync(patientId)).ReturnsAsync(existingPatientAppointments);
        _appointmentRepoMock.Setup(x => x.GetAllByPractitionerIdAsync(practitionerId)).ReturnsAsync(existingPractitionerAppointments);
        _appointmentRepoMock.Setup(x => x.GetAllByClinicIdAsync(clinicId)).ReturnsAsync(new List<Appointment>());
        _appointmentRepoMock.Setup(x => x.AddAsync(It.IsAny<Appointment>())).Returns(Task.CompletedTask);

        // Act
        await _sut.Execute(request);

        // Assert
        _appointmentRepoMock.Verify(x => x.AddAsync(It.IsAny<Appointment>()), Times.Once);
        _appointmentRepoMock.Verify(x => x.SaveAsync(), Times.Once);
    }
}
