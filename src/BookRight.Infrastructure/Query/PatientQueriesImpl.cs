using System;
using System.Collections.Generic;
using System.Text;
using BookRight.Facade.DTO;
using BookRight.Facade.Interfaces.Queries;
using Microsoft.EntityFrameworkCore;

namespace BookRight.Infrastructure.Query
{
    public class PatientQueriesImpl : IPatientQueries
    {
        private readonly AppDbContext _db;
        public PatientQueriesImpl(AppDbContext db)
        {
            _db = db;
        }

        //Henter alle patienter fra databasen og returnerer dem som en read-only liste af DTO'er
        async Task<IReadOnlyList<PatientDto>> IPatientQueries.GetAllAsync()
        {
            return await _db.Patients.AsNoTracking().Select(p => new PatientDto(
                p.Id,
                p.FirstName,
                p.LastName,
                p.Email,
                p.PhoneNumber,
                p.Birthday
                )).ToListAsync();
        }

        // Henter en enkelt patient med detaljerede oplysninger baseret på id
        // Returnerer null hvis patienten ikke findes
        async Task<PatientDetailedDto?> IPatientQueries.GetAsync(Guid id)
        {
            return await _db.Patients.AsNoTracking()
                .Where(p => p.Id == id).Select(p => new PatientDetailedDto(
                    p.Id,
                    p.FirstName,
                    p.LastName,
                    p.Email,
                    p.PhoneNumber,
                    p.Birthday,
                    p.PatientAddress.StreetName,
                    p.PatientAddress.Zipcode,
                    p.Note,
                    p.PreferredPractitioner
                )).FirstOrDefaultAsync();
        }

    }
}
