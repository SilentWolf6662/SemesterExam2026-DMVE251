using System;
using System.Collections.Generic;
using System.Text;
using BookRight.Facade.DTO;

namespace BookRight.Facade.Interfaces.Queries
{
    public interface IClinicQueries
    {
        Task<ClinicDto?> GetAsync(Guid id);
        Task<IReadOnlyList<ClinicDto>> GetAllAsync();
    }
}
