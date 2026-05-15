using System;
using System.Collections.Generic;
using System.Text;
using BookRight.Facade.DTO;

namespace BookRight.Facade.Interfaces.Queries
{
    public interface IPractitionerQueries
    {
        Task<PractitionerDto?> GetAsync(Guid id);
        Task<IReadOnlyList<PractitionerDto>> GetAllAsync();
        
    }
}
