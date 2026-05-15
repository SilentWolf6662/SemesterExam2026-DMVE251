using BookRight.Domain.Entities;
using BookRight.Facade.Command;
using BookRight.UseCases.Repositories;
using BookRight.UseCases.Service;
using Moq;

namespace BookRight.UseCases.Tests;

public class BookAppointmentTests
{
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
    private readonly Mock<IPractitionerRepository> _practitionerRepoMock;
    private readonly Mock<IPatientRepository> _patientRepoMock;
    private readonly Mock<IClinicRepository> _clinicRepoMock;
    private readonly BookAppointment _sut; // _sut står for "System Under Test", og er en konvention for at navngive den klasse, som vi tester

    public BookAppointmentTests()
    {
        _appointmentRepoMock = new Mock<IAppointmentRepository>();
        _practitionerRepoMock = new Mock<IPractitionerRepository>();
        _patientRepoMock = new Mock<IPatientRepository>();
        _clinicRepoMock = new Mock<IClinicRepository>();
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

        _patientRepoMock.Setup(x => x.GetByIdAsync(patientId)).ReturnsAsync(mockPatient); // Returnere en mock patient
        _practitionerRepoMock.Setup(x => x.GetByIdAsync(practitionerId)).ReturnsAsync(mockPractitioner); // Returnere en mock practitioner

        _appointmentRepoMock.Setup(x => x.GetAllByPatientIdAsync(patientId)).ReturnsAsync(existingPatientAppointments); // Returnere en tom liste for patientens eksisterende aftaler
        _appointmentRepoMock.Setup(x => x.GetAllByPractitionerIdAsync(practitionerId)).ReturnsAsync(existingPractitionerAppointments); // Returnere en tom liste for practitionerens eksisterende aftaler
        _appointmentRepoMock.Setup(x => x.AddAsync(It.IsAny<Appointment>())).Returns(Task.CompletedTask); // Simulerer, at tilføjelsen af en aftale lykkes

        // Act
        await _sut.Execute(request);

        // Assert
        _patientRepoMock.Verify(x => x.GetByIdAsync(patientId), Times.Once); // Verificerer, at GetPatient_ByIdAsync blev kaldt én gang med det forventede patientId
        _practitionerRepoMock.Verify(x => x.GetByIdAsync(practitionerId), Times.Once); // Verificerer, at GetPractitioner_ByIdAsync blev kaldt én gang med det forventede practitionerId
        _appointmentRepoMock.Verify(x => x.GetByIdAsync(patientId), Times.Once); // Verificerer, at GetAppointments_ByPatientIdAsync blev kaldt én gang med det forventede patientId
        _appointmentRepoMock.Verify(x => x.GetByIdAsync(practitionerId), Times.Once); // Verificerer, at GetAppointments_ByPractitionerIdAsync blev kaldt én gang med det forventede practitionerId
        _appointmentRepoMock.Verify(x => x.AddAsync(It.IsAny<Appointment>()), Times.Once); // Verificerer, at AddAppointmentAsync blev kaldt én gang
        //Assert.NotEmpty(await IAppointmentQueries.GetAllAsync().Result);
    }
}
