using BookRight.Facade.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookRight.Facade.Interfaces.Queries
{
    public interface IClinicQueries
    {
        Task<ClinicDto?> GetAsync(Guid id);
        Task<IReadOnlyList<ClinicDto>> GetAllAsync();
    }
}
