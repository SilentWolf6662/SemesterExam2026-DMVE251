using BookRight.Domain.Entities;
using BookRight.Domain.Exceptions;
using BookRight.Domain.ValueObjects;
using BookRight.Facade.Command;
using BookRight.Facade.Interfaces.Queries;
using BookRight.UseCases.Repositories;
using BookRight.UseCases.Service;
using Moq;
using Xunit;

namespace BookRight.UseCases.Tests;

public class BookAppointmentTests
{
    private readonly Mock<IAppointmentRepo> _appointmentRepoMock;
    private readonly Mock<IPractitionerRepo> _practitionerRepoMock;
    private readonly Mock<IPatientRepo> _patientRepoMock;
    private readonly Mock<IClinicRepo> _clinicRepoMock;
    private readonly BookAppointment _sut; // _sut står for "System Under Test", og er en konvention for at navngive den klasse, som vi tester

    public BookAppointmentTests()
    {
        _appointmentRepoMock = new Mock<IAppointmentRepo>();
        _practitionerRepoMock = new Mock<IPractitionerRepo>();
        _patientRepoMock = new Mock<IPatientRepo>();
        _clinicRepoMock = new Mock<IClinicRepo>();
        _sut = new BookAppointment(_appointmentRepoMock.Object, _practitionerRepoMock.Object, _patientRepoMock.Object, _clinicRepoMock.Object);
    }

    [Fact]
    public async Task Execute_ValidRequest_AddsAppointmentSuccessfully()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var practitionerId = Guid.NewGuid();
        var treatmentTypeId = Guid.NewGuid();
        var start = DateTime.UtcNow.AddDays(1); // Brug tidspunktet nu og tilføj en dag, så det er i fremtiden
        var end = start.AddHours(1); // Lig 1 time på start, så end er senere

        var request = new BookAppointmentRequest(start, end, treatmentTypeId, patientId, practitionerId);
        
        var mockPatient = Mock.Of<Patient>();
        var mockPractitioner = Mock.Of<Practitioner>();
        var existingPatientAppointments = new List<Appointment>();
        var existingPractitionerAppointments = new List<Appointment>();

        _patientRepoMock.Setup(x => x.GetPatient_ByIdAsync(patientId)).ReturnsAsync(mockPatient); // Returnere en mock patient
        _practitionerRepoMock.Setup(x => x.GetPractitioner_ByIdAsync(practitionerId)).ReturnsAsync(mockPractitioner); // Returnere en mock practitioner

        _appointmentRepoMock.Setup(x => x.GetAppointments_ByPatientIdAsync(patientId)).ReturnsAsync(existingPatientAppointments); // Returnere en tom liste for patientens eksisterende aftaler
        _appointmentRepoMock.Setup(x => x.GetAppointments_ByPractitionerIdAsync(practitionerId)).ReturnsAsync(existingPractitionerAppointments); // Returnere en tom liste for practitionerens eksisterende aftaler
        _appointmentRepoMock.Setup(x => x.AddAppointmentAsync(It.IsAny<Appointment>())).Returns(Task.CompletedTask); // Simulerer, at tilføjelsen af en aftale lykkes

        // Act
        await _sut.Execute(request);

        // Assert
        _patientRepoMock.Verify(x => x.GetPatient_ByIdAsync(patientId), Times.Once); // Verificerer, at GetPatient_ByIdAsync blev kaldt én gang med det forventede patientId
        _practitionerRepoMock.Verify(x => x.GetPractitioner_ByIdAsync(practitionerId), Times.Once); // Verificerer, at GetPractitioner_ByIdAsync blev kaldt én gang med det forventede practitionerId
        _appointmentRepoMock.Verify(x => x.GetAppointments_ByPatientIdAsync(patientId), Times.Once); // Verificerer, at GetAppointments_ByPatientIdAsync blev kaldt én gang med det forventede patientId
        _appointmentRepoMock.Verify(x => x.GetAppointments_ByPractitionerIdAsync(practitionerId), Times.Once); // Verificerer, at GetAppointments_ByPractitionerIdAsync blev kaldt én gang med det forventede practitionerId
        _appointmentRepoMock.Verify(x => x.AddAppointmentAsync(It.IsAny<Appointment>()), Times.Once); // Verificerer, at AddAppointmentAsync blev kaldt én gang
        //Assert.NotEmpty(await IAppointmentQueries.GetAllAsync().Result);
    }
}
