using BookRight.Facade.DTO;

namespace BookRight.Facade.Interfaces.Queries;

public interface ITreatmentTypeQueries
{
    Task<TreatmentTypeDto?> GetAsync(Guid id);
    Task<IReadOnlyList<TreatmentTypeDto>> GetAllAsync();
        
}