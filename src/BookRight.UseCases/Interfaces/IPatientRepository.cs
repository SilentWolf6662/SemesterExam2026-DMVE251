using BookRight.Domain.Entities;

namespace BookRight.UseCases.Repositories;

public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(Guid patientId);
    Task SaveAsync();
    Task AddAsync(Patient patient);
}