using BookRight.Facade.DTO;

namespace BookRight.Facade.Interfaces.Queries;

public interface IClinicOccupancyQueries
{
    Task<IReadOnlyList<ClinicOccupancyDto>> GetTodayAsync();
}
