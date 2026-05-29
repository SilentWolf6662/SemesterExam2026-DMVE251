using BookRight.Facade.DTO;
using BookRight.Facade.Interfaces.Queries;
using Microsoft.EntityFrameworkCore;

namespace BookRight.Infrastructure.Query;

public class ClinicQueriesImpl : IClinicQueries
{
    private readonly AppDbContext _db;

    public ClinicQueriesImpl(AppDbContext db)
    {
        _db = db;
    }

    //Henter alle Klinikker fra databasen og returnerer dem som en read-only liste af DTO'er
    async Task<IReadOnlyList<ClinicDto>> IClinicQueries.GetAllAsync()
    {
        return await _db.Clinics.AsNoTracking().Select(c => new ClinicDto(
            c.Id,
            c.ClinicAddress.StreetName,
            c.ClinicAddress.Zipcode,
            c.Rooms,
            c.WorkingHours.Select(w => new TimeIntervalDto(w.Start, w.End)).ToList()
        )).ToListAsync();
    }

    // Henter en enkelt Klinik med detaljerede oplysninger baseret på id
    // Returnerer null hvis Klinikken ikke findes
    async Task<ClinicDto?> IClinicQueries.GetAsync(Guid id)
    {
        return await _db.Clinics.AsNoTracking()
            .Where(c => c.Id == id).Select(c => new ClinicDto(
                c.Id,
                c.ClinicAddress.StreetName,
                c.ClinicAddress.Zipcode,
                c.Rooms,
                c.WorkingHours.Select(w => new TimeIntervalDto(w.Start, w.End)).ToList()
            )).FirstOrDefaultAsync();
    }
       

}