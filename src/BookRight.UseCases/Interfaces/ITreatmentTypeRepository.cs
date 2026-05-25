using BookRight.Domain.Entities;

namespace BookRight.UseCases.Repositories;

public interface ITreatmentTypeRepository
{
    Task<TreatmentType?> GetByIdAsync(Guid treatmentTypeId);
    Task SaveAsync();
    Task AddAsync(TreatmentType treatmentType);
}