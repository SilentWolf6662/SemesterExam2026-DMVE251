using BookRight.Domain.Entities;
using BookRight.Facade.DTO;
using BookRight.Facade.Interfaces.Queries;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace BookRight.Infrastructure.Query
{
    public class TreatmentTypeQueriesImpl : ITreatmentTypeQueries
    {
        private readonly AppDbContext _db;

        public TreatmentTypeQueriesImpl(AppDbContext db)
        {
            _db = db;
        }

        async Task<IReadOnlyList<TreatmentTypeDto>> ITreatmentTypeQueries.GetAllAsync()
        {
            return await _db.TreatmentTypes.AsNoTracking().Select(t => new TreatmentTypeDto(
               t.Id,
               t.Name,
               t.AuthorizationType.ToString(),
               t.MaxParticipants
                )).ToListAsync();
        }

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
}
