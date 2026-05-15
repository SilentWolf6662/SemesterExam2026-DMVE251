using BookRight.Domain.Entities;
using BookRight.UseCases.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookRight.Infrastructure.Repository;

public class PatientRepo : IPatientRepo
{
    private readonly AppDbContext _db;

    public PatientRepo(AppDbContext db)
    {
        _db = db;
    }

    Task<Patient?> IPatientRepo.GetPatient_ByIdAsync(Guid id)
    {
        return _db.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    Task IPatientRepo.AddPatientAsync(Patient patient)
    {
        _db.Patients.Add(patient);
        return _db.SaveChangesAsync();
    }
}
