using BookRight.Facade.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookRight.Facade.Interfaces.Queries
{
    public interface IAppointmentDetailedQueries
    {
        Task<AppointmentDetailedDto?> GetAsync(Guid id);
        Task<IReadOnlyList<AppointmentDetailedDto>> GetAllAsync();
    }
}
