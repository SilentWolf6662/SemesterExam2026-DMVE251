using BookRight.Domain.Entities;

namespace BookRight.UseCases.Repositories;

public interface IClinicRepository
{
    Task<Clinic?> GetByIdAsync(Guid clinicId);
    Task SaveAsync();
    Task AddAsync(Clinic clinic);
}