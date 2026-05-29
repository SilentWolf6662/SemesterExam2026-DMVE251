using BookRight.Facade.DTO;

namespace BookRight.Facade.Interfaces.Queries;

public interface IAppointmentQueries
{
    Task<AppointmentDetailedDto?> GetAsync(Guid id);
    Task<IReadOnlyList<AppointmentDto>> GetAllAsync();
}