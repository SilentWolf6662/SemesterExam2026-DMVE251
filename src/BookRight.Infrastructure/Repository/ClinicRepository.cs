using BookRight.Domain.Entities;
using BookRight.UseCases.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookRight.Infrastructure.Repository;

public class ClinicRepository : IClinicRepository
{
    private readonly AppDbContext _db;

    public ClinicRepository(AppDbContext db)
    {
        _db = db;
    }

    async Task<Clinic?> IClinicRepository.GetByIdAsync(Guid clinicId)
    {
        return await _db.Clinics
            .FirstOrDefaultAsync(b => b.Id == clinicId);
    }

    Task IClinicRepository.SaveAsync()
    {
        return _db
            .SaveChangesAsync();
    }

    async Task IClinicRepository.AddAsync(Clinic clinic)
    {
        await _db.Clinics
            .AddAsync(clinic);
    }
}