using BookRight.Domain.Entities;
using BookRight.UseCases.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookRight.Infrastructure.Repository;

public class PractitionerRepo : IPractitionerRepo
{
    private readonly AppDbContext _db;

    public PractitionerRepo(AppDbContext db)
    {
        _db = db;
    }

    Task<Practitioner?> IPractitionerRepo.GetPractitioner_ByIdAsync(Guid id)
    {
        return _db.Practitioners
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    Task IPractitionerRepo.AddPractitionerAsync(Practitioner practitioner)
    {
        _db.Practitioners.Add(practitioner);
        return _db.SaveChangesAsync();
    }
}
