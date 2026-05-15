using BookRight.Domain.Entities;
using BookRight.Domain.Exceptions;
using BookRight.Domain.ValueObjects;
using BookRight.Facade.Command;
using BookRight.UseCases.Repositories;

namespace BookRight.UseCases.Service;

public class BookAppointment
{
    private readonly IAppointmentRepo _appointmentRepo;
    private readonly IPractitionerRepo _practitionerRepo;
    private readonly IPatientRepo _patientRepo;
    private readonly IClinicRepo _clinicRepo;

    public BookAppointment(IAppointmentRepo appointmentRepo, IPractitionerRepo practitionerRepo, IPatientRepo patientRepo, IClinicRepo clinicRepo)
    {
        _appointmentRepo = appointmentRepo;
        _practitionerRepo = practitionerRepo;
        _patientRepo = patientRepo;
        _clinicRepo = clinicRepo;
    }

    public async Task Execute(BookAppointmentRequest request)
    {
        // Tjek om vores patient og practitioner findes, hvis de ikke findes skal vi stoppe processen og kaste en NotFoundException
        _ = await _patientRepo.GetPatient_ByIdAsync(request.PatientId) // Tjek om patient findes
            ?? throw new NotFoundException("Patient not found"); // Hvis ikke, kast en NotFoundException

        _ = await _practitionerRepo.GetPractitioner_ByIdAsync(request.PractitionerId) // Tjek om practitioner findes
            ?? throw new NotFoundException("Practitioner not found"); // Hvis ikke, kast en NotFoundException

        // Lig tidsintervallet ind i en variable, så den er nemmere at arbejde med og nemmere at læse
        var timeInterval = new TimeInterval(request.From, request.To);

        // Hent alle eksisterende bookinger for både patient og practitioner, så vi kan tjekke for overlap
        var patientBookinger = await _appointmentRepo.GetAppointments_ByPatientIdAsync(request.PatientId);
        var practitionerBookinger = await _appointmentRepo.GetAppointments_ByPractitionerIdAsync(request.PractitionerId);

        // Opret en ny appointment ved at kalde Create-factory metoden på Appointment.cs, og send alle nødvendige informationer med
        var appointment = Appointment.Create(
            timeInterval, 
            request.TreatmentTypeId, 
            request.PatientId, 
            request.PractitionerId, 
            patientBookinger, 
            practitionerBookinger);

        // Tilføj den nye appointment til vores repository, så den bliver gemt i databasen
        await _appointmentRepo.AddAppointmentAsync(appointment);
    }
}