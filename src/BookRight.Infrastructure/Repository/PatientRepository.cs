using BookRight.Domain.Entities;
using BookRight.UseCases.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookRight.Infrastructure.Repository;

public class PatientRepository : IPatientRepository
{
    private readonly AppDbContext _db;

    public PatientRepository(AppDbContext db)
    {
        _db = db;
    }

    Task<Patient?> IPatientRepository.GetByIdAsync(Guid patientId)
    {
        return _db.Patients
            .FirstOrDefaultAsync(b => b.Id == patientId);
    }

    Task IPatientRepository.SaveAsync()
    {
        return _db
            .SaveChangesAsync();
    }

    async Task IPatientRepository.AddAsync(Patient patient)
    {
        await _db.Patients
            .AddAsync(patient);
    }
}