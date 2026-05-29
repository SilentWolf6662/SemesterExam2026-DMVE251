using BookRight.Domain.Entities;
using BookRight.UseCases.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookRight.Infrastructure.Repository;

public class PractitionerRepository : IPractitionerRepository
{
    private readonly AppDbContext _db;

    public PractitionerRepository(AppDbContext db)
    {
        _db = db;
    }

    Task<Practitioner?> IPractitionerRepository.GetByIdAsync(Guid practitionerId)
    {
        return _db.Practitioners
            .FirstOrDefaultAsync(p => p.Id == practitionerId);
    }

    Task IPractitionerRepository.SaveAsync()
    {
        return _db.SaveChangesAsync();
    }

    async Task IPractitionerRepository.AddAsync(Practitioner practitioner)
    {
        await _db.Practitioners.AddAsync(practitioner);
    }
}
