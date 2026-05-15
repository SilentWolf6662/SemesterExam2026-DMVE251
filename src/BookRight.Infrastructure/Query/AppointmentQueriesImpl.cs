using BookRight.Facade.Interfaces.Queries;
using BookRight.Facade.DTO;
using Microsoft.EntityFrameworkCore;

namespace BookRight.Infrastructure.Query;

public class AppointmentQueriesImpl : IAppointmentQueries
{
    private readonly AppDbContext _db;

    public AppointmentQueriesImpl(AppDbContext db)
    {
        _db = db;
    }

    //Henter alle aftaler fra databasen og returnerer dem som en read-only liste af DTO'er
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


    // Henter en enkelt aftale med detaljerede oplysninger baseret på id
    // Returnerer null hvis aftalen ikke findes
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