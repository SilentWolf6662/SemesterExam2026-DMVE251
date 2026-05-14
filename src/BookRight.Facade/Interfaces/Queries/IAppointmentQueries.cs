using BookRight.Facade.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookRight.Facade.Interfaces.Queries
{
    public interface IAppointmentQueries
    {
        Task<AppointmentDto?> GetAsync(Guid id);
        Task<IReadOnlyList<AppointmentDto>> GetAllAsync();
    }
}
