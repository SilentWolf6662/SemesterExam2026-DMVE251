using BookRight.Facade.DTO;
using BookRight.Facade.Interfaces.Queries;
using Microsoft.EntityFrameworkCore;

namespace BookRight.Infrastructure.Query;

public class TreatmentTypeQueriesImpl : ITreatmentTypeQueries
{
    private readonly AppDbContext _db;

    public TreatmentTypeQueriesImpl(AppDbContext db)
    {
        _db = db;
    }

    //Henter alle Treatmentstypes fra databasen og returnerer dem som en read-only liste af DTO'er
    async Task<IReadOnlyList<TreatmentTypeDto>> ITreatmentTypeQueries.GetAllAsync()
    {
        return await _db.TreatmentTypes.AsNoTracking().Select(t => new TreatmentTypeDto(
            t.Id,
            t.Name,
            t.AuthorizationType.ToString(),
            t.MaxParticipants
        )).ToListAsync();
    }


    // Henter en enkelt Treatmenttype med detaljerede oplysninger baseret på id
    // Returnerer null hvis Treatmenttypen ikke findes
    async Task<TreatmentTypeDto?> ITreatmentTypeQueries.GetAsync(Guid id)
    {
        return await _db.TreatmentTypes.AsNoTracking()
            .Where(t => t.Id == id).Select(t => new TreatmentTypeDto(
                t.Id,
                t.Name,
                t.AuthorizationType.ToString(),
                t.MaxParticipants
            )).FirstOrDefaultAsync();
    }

}