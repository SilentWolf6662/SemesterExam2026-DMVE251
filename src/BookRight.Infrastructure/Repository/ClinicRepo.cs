using BookRight.Domain.Entities;
using BookRight.UseCases.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookRight.Infrastructure.Repository;

public class ClinicRepo : IClinicRepo
{
    private readonly AppDbContext _db;

    public ClinicRepo(AppDbContext db)
    {
        _db = db;
    }

    Task<Clinic?> IClinicRepo.GetClinic_ByIdAsync(Guid id)
    {
        return _db.Clinics
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    Task IClinicRepo.AddClinicAsync(Clinic clinic)
    {
        _db.Clinics.Add(clinic);
        return _db.SaveChangesAsync();
    }
}
