using BookRight.Facade.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookRight.Facade.Interfaces.Queries
{
    public interface IAppointmentQueries
    {
        Task<BookAppointmentRequest?> GetAsync(Guid id);
        Task<IReadOnlyList<BookAppointmentRequest>> GetAllAsync();
    }
}
