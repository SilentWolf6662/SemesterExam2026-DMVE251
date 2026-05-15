using BookRight.Facade.Interfaces.Queries;
using BookRight.Facade.DTO;
using Microsoft.EntityFrameworkCore;

namespace BookRight.Infrastructure.Query;

public class AppointmentQueriesImpl : IAppointmentQueries
{
    private readonly AppDbContext _db;

    AppointmentQueriesImpl(AppDbContext db)
    {
        _db = db;
    }

    //Henter og læser kun informationer fra databasen omkring Appointment
    async Task<IReadOnlyList<AppointmentDto>> IAppointmentQueries.GetAllAsync()
    {
        return await _db.Appointments.AsNoTracking().Select(a => new AppointmentDto(
            a.Id,
            a.TimeInterval.Start,
            a.TimeInterval.End,
            a.TreatmentTypeId,
            a.PractitionerId,
            a.Status.ToString()
            )).ToListAsync();
    }


    // Henter informationer fra databasen omkring Appointment
    // Bruger AsNoTracking så EF-Core ikke tracker den
    async Task<AppointmentDetailedDto?> IAppointmentQueries.GetAsync(Guid id)
    {
        return await _db.Appointments.AsNoTracking()
            .Where(a => a.Id == id).Select(a => new AppointmentDetailedDto(
            a.Id,
            a.TimeInterval.Start,
            a.TimeInterval.End,
            a.TreatmentTypeId,
            a.PatientId,
            a.PractitionerId,
            a.Status.ToString(),
            a.Note
            )).FirstOrDefaultAsync();
    }
}