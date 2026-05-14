using BookRight.UseCases.Repositories;
using BookRight.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookRight.Infrastructure.Repository;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppDbContext _db;

    public AppointmentRepository(AppDbContext db)
    {
        _db = db;
    }

    // Henter en appointment baseret på dens Id
    async Task<Appointment?> IAppointmentRepository.GetAppointment_ByIdAsync(Guid id)
    {
        return await _db.Appointments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    // Henter alle appointments baseret på practitioner (behandler) Id
    async Task<IReadOnlyList<Appointment>> IAppointmentRepository.GetAppointments_ByPractitionerIdAsync(Guid practitionerId)
    {
        return await _db.Appointments
            .AsNoTracking()
            .Where(a => a.PractitionerId == practitionerId)
            .ToListAsync();
    }

    // Henter alle appointments baseret på patient Id
    async Task<IReadOnlyList<Appointment>> IAppointmentRepository.GetAppointments_ByPatientIdAsync(Guid patientId)
    {
        return await _db.Appointments
            .AsNoTracking()
            .Where(a => a.PatientId == patientId)
            .ToListAsync();
    }

    // Tilføjer en ny appointment til databasen
    Task IAppointmentRepository.AddAppointmentAsync(Appointment appointment)
    {
        _db.Appointments.Add(appointment);
        return _db.SaveChangesAsync();
    }

    // Opdaterer en eksisterende appointment i databasen
    Task IAppointmentRepository.UpdateAppointmentStatus_ToCompletedAsync(Appointment appointment)
    {
        //_db.Appointments.Update(appointment.Complete());
        return _db.SaveChangesAsync();
    }
    Task IAppointmentRepository.UpdateAppointmentStatus_ToCancelledAsync(Appointment appointment)
    {
        //_db.Appointments.Update(appointment.Cancel());
        return _db.SaveChangesAsync();
    }
    Task IAppointmentRepository.UpdateAppointmentStatus_ToNoShowAsync(Appointment appointment)
    {
        //_db.Appointments.Update(appointment.NoOneShowed());
        return _db.SaveChangesAsync();
    }
}