using Microsoft.EntityFrameworkCore;
﻿using BookRight.Domain.Entities;
using BookRight.UseCases.Repositories;

namespace BookRight.Infrastructure.Repository;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppDbContext _db;

    public AppointmentRepository(AppDbContext db)
    {
        _db = db;
    }

    // Henter en appointment baseret på dens Id
    async Task<Appointment?> IAppointmentRepository.GetByIdAsync(Guid id)
    {
        return await _db.Appointments
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    // Henter alle appointments baseret på practitioner (behandler) Id
    async Task<IReadOnlyList<Appointment>> IAppointmentRepository.GetAllByPractitionerIdAsync(Guid practitionerId)
    {
        return await _db.Appointments
            .AsNoTracking()
            .Where(a => a.PractitionerId == practitionerId)
            .ToListAsync();
    }

    // Henter alle appointments baseret på patient Id
    async Task<IReadOnlyList<Appointment>> IAppointmentRepository.GetAllByPatientIdAsync(Guid patientId)
    {
        return await _db.Appointments
            .AsNoTracking()
            .Where(a => a.PatientId == patientId)
            .ToListAsync();
    }

    // Tilføjer en ny appointment til databasen
    Task IAppointmentRepository.SaveAsync()
    {
        return _db
            .SaveChangesAsync();
    }

    // Gemmer i database
    async Task IAppointmentRepository.AddAsync(Appointment appointment)
    {
        await _db.Appointments
            .AddAsync(appointment);
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