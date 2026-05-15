using BookRight.Domain.Entities;
using BookRight.Domain.Exceptions;
using BookRight.Domain.ValueObjects;
using BookRight.Facade.Command;
using BookRight.UseCases.Repositories;

namespace BookRight.UseCases.Service;

public class BookAppointment
{
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly IPractitionerRepository _practitionerRepo;
    private readonly IPatientRepository _patientRepo;
    private readonly IClinicRepository _clinicRepo;

    public BookAppointment(IAppointmentRepository appointmentRepo, IPractitionerRepository practitionerRepo, IPatientRepository patientRepo, IClinicRepository clinicRepo)
    {
        _appointmentRepo = appointmentRepo;
        _practitionerRepo = practitionerRepo;
        _patientRepo = patientRepo;
        _clinicRepo = clinicRepo;
    }

    public async Task Execute(BookAppointmentRequest request)
    {
        // Tjek om vores patient og practitioner findes, hvis de ikke findes skal vi stoppe processen og kaste en NotFoundException
        _ = await _patientRepo.GetByIdAsync(request.PatientId) // Tjek om patient findes
            ?? throw new NotFoundException("Patient not found"); // Hvis ikke, kast en NotFoundException

        _ = await _practitionerRepo.GetByIdAsync(request.PractitionerId) // Tjek om practitioner findes
            ?? throw new NotFoundException("Practitioner not found"); // Hvis ikke, kast en NotFoundException

        // Lig tidsintervallet ind i en variable, så den er nemmere at arbejde med og nemmere at læse
        var timeInterval = new TimeInterval(request.From, request.To);

        // Hent alle eksisterende bookinger for både patient og practitioner, så vi kan tjekke for overlap
        var patientBookinger = await _appointmentRepo.GetAllByPatientIdAsync(request.PatientId);
        var practitionerBookinger = await _appointmentRepo.GetAllByPractitionerIdAsync(request.PractitionerId);

        // Opret en ny appointment ved at kalde Create-factory metoden på Appointment.cs, og send alle nødvendige informationer med
        var appointment = Appointment.Create(
            timeInterval, 
            request.TreatmentTypeId, 
            request.PatientId, 
            request.PractitionerId, 
            patientBookinger, 
            practitionerBookinger);

        // Tilføj den nye appointment til vores repository, så den bliver gemt i databasen
        await _appointmentRepo.AddAsync(appointment);
        await _appointmentRepo.SaveAsync();
    }
}