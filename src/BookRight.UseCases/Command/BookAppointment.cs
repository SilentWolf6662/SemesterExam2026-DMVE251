using BookRight.Domain.Entities;
using BookRight.Domain.Exceptions;
using BookRight.Domain.ValueObjects;
using BookRight.Facade.Command;
using BookRight.Facade.Interfaces.UseCase;
using BookRight.UseCases.Repositories;

namespace BookRight.UseCases.Command;

public class BookAppointment : IBookAppointment
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IPractitionerRepository _practitionerRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IClinicRepository _clinicRepository;

    public BookAppointment(IAppointmentRepository appointmentRepository, IPractitionerRepository practitionerRepository, IPatientRepository patientRepository, IClinicRepository clinicRepository)
    {
        _appointmentRepository = appointmentRepository;
        _practitionerRepository = practitionerRepository;
        _patientRepository = patientRepository;
        _clinicRepository = clinicRepository;
    }

    async Task IBookAppointment.Execute(BookAppointmentRequest request)
    {
        // Tjek om vores patient og practitioner findes, hvis de ikke findes skal vi stoppe processen og kaste en NotFoundException
        _ = await _patientRepository.GetByIdAsync(request.PatientId) // Tjek om patient findes
            ?? throw new NotFoundException("Patient not found"); // Hvis ikke, kast en NotFoundException

        _ = await _practitionerRepository.GetByIdAsync(request.PractitionerId) // Tjek om practitioner findes
            ?? throw new NotFoundException("Practitioner not found"); // Hvis ikke, kast en NotFoundException

        // Lig tidsintervallet ind i en variable, så den er nemmere at arbejde med og nemmere at læse
        var timeInterval = new TimeInterval(request.From, request.To);

        // Hent alle eksisterende bookinger for både patient og practitioner, så vi kan tjekke for overlap
        var patientBookinger = await _appointmentRepository.GetAllByPatientIdAsync(request.PatientId);
        var practitionerBookinger = await _appointmentRepository.GetAllByPractitionerIdAsync(request.PractitionerId);

        // Opret en ny appointment ved at kalde Create-factory metoden på Appointment.cs, og send alle nødvendige informationer med
        var appointment = Appointment.Create(
            timeInterval, 
            request.TreatmentTypeId, 
            request.PatientId, 
            request.PractitionerId, 
            patientBookinger, 
            practitionerBookinger);

        // Tilføj den nye appointment til vores repository, så den bliver gemt i databasen
        await _appointmentRepository.AddAsync(appointment);
        await _appointmentRepository.SaveAsync();
    }
}