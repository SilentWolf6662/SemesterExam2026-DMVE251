using BookRight.Domain.Entities;

namespace BookRight.UseCases.Repositories;

public interface IPractitionerRepository
{
    Task<Practitioner?> GetByIdAsync(Guid practitionerId);
    Task SaveAsync();
    Task AddAsync(Practitioner practitioner);
}
