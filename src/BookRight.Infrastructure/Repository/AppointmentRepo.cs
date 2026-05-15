using BookRight.Domain.Entities;
using BookRight.UseCases.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookRight.Infrastructure.Repository;

public class AppointmentRepo : IAppointmentRepo
{
    private readonly AppDbContext _db;

    public AppointmentRepo(AppDbContext db)
    {
        _db = db;
    }

    // Henter en appointment baseret på dens Id
    async Task<Appointment?> IAppointmentRepo.GetAppointment_ByIdAsync(Guid id)
    {
        return await _db.Appointments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    // Henter alle appointments baseret på practitioner (behandler) Id
    async Task<IReadOnlyList<Appointment>> IAppointmentRepo.GetAppointments_ByPractitionerIdAsync(Guid practitionerId)
    {
        return await _db.Appointments
            .AsNoTracking()
            .Where(a => a.PractitionerId == practitionerId)
            .ToListAsync();
    }

    // Henter alle appointments baseret på patient Id
    async Task<IReadOnlyList<Appointment>> IAppointmentRepo.GetAppointments_ByPatientIdAsync(Guid patientId)
    {
        return await _db.Appointments
            .AsNoTracking()
            .Where(a => a.PatientId == patientId)
            .ToListAsync();
    }

    // Tilføjer en ny appointment til databasen
    Task IAppointmentRepo.AddAppointmentAsync(Appointment appointment)
    {
        _db.Appointments.Add(appointment);
        //_db.Add(appointment);
        return _db.SaveChangesAsync();
    }

    // Opdaterer en eksisterende appointment i databasen
    //Task IAppointmentRepository.UpdateAppointmentStatus_ToCompletedAsync(Appointment appointment)
    //{
    //    //_db.Appointments.Update(appointment.Complete());
    //    return _db.SaveChangesAsync();
    //}
    //Task IAppointmentRepository.UpdateAppointmentStatus_ToCancelledAsync(Appointment appointment)
    //{
    //    //_db.Appointments.Update(appointment.Cancel());
    //    return _db.SaveChangesAsync();
    //}
    //Task IAppointmentRepository.UpdateAppointmentStatus_ToNoShowAsync(Appointment appointment)
    //{
    //    //_db.Appointments.Update(appointment.NoOneShowed());
    //    return _db.SaveChangesAsync();
    //}
}