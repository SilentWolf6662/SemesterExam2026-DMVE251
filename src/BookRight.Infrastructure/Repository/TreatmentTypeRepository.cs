using BookRight.Domain.Entities;
using BookRight.UseCases.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookRight.Infrastructure.Repository;

public class TreatmentTypeRepository : ITreatmentTypeRepository
{
    private readonly AppDbContext _db;

    public TreatmentTypeRepository(AppDbContext db)
    {
        _db = db;
    }

    Task<TreatmentType?> ITreatmentTypeRepository.GetByIdAsync(Guid treatmentTypeId)
    {
        return _db.TreatmentTypes
            .FirstOrDefaultAsync(b => b.Id == treatmentTypeId);
    }

    Task ITreatmentTypeRepository.SaveAsync()
    {
        return _db
            .SaveChangesAsync();
    }

    async Task ITreatmentTypeRepository.AddAsync(TreatmentType treatmentType)
    {
        await _db.TreatmentTypes
            .AddAsync(treatmentType);
    }
}