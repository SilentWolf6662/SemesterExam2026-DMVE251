using System;
using System.Collections.Generic;
using System.Text;
using BookRight.Facade.DTO;
using BookRight.Facade.Interfaces.Queries;
using Microsoft.EntityFrameworkCore;

namespace BookRight.Infrastructure.Query
{
    public class PractitionerQueriesImpl : IPractitionerQueries
    {
        private readonly AppDbContext _db;

        public PractitionerQueriesImpl(AppDbContext db)
        {
            _db = db;
        }

        //Henter alle Practitioner fra databasen og returnerer dem som en read-only liste af DTO'er
        async Task<IReadOnlyList<PractitionerDto>> IPractitionerQueries.GetAllAsync()
        {
            return await _db.Practitioners.AsNoTracking().Select(p => new PractitionerDto(
                p.Id,
                p.FirstName,
                p.LastName,
                p.Email,
                p.PhoneNumber,
                p.Authorization.ToString(),
                p.AuthorizationNumber,
                p.Clinics
                )).ToListAsync();
        }

        // Henter en enkelt Practitioner med detaljerede oplysninger baseret på id
        // Returnerer null hvis Practioner ikke findes
        async Task<PractitionerDto?> IPractitionerQueries.GetAsync(Guid id)
        {
            return await _db.Practitioners.AsNoTracking()
                .Where(p => p.Id == id).Select(p => new PractitionerDto(
                    p.Id,
                    p.FirstName,
                    p.LastName,
                    p.Email,
                    p.PhoneNumber,
                    p.Authorization.ToString(),
                    p.AuthorizationNumber,
                    p.Clinics
                    )).FirstOrDefaultAsync();
        }

    }
}
