using BookRight.Facade.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookRight.Facade.Interfaces.Queries
{
    public interface IPractitionerQueries
    {
        Task<PractitionerDto?> GetAsync(Guid id);
        Task<IReadOnlyList<PractitionerDto>> GetAllAsync();
        
    }
}
