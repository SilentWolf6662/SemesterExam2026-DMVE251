using BookRight.Facade.DTO;

namespace BookRight.Facade.Interfaces.Queries;

public interface IClinicQueries
{
    Task<ClinicDto?> GetAsync(Guid id);
    Task<IReadOnlyList<ClinicDto>> GetAllAsync();
}