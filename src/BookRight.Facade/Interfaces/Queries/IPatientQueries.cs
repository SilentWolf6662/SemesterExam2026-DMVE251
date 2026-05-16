using BookRight.Facade.DTO;

namespace BookRight.Facade.Interfaces.Queries;

public interface IPatientQueries
{
    Task<PatientDetailedDto?> GetAsync(Guid id);
    Task<IReadOnlyList<PatientDto>> GetAllAsync();
}