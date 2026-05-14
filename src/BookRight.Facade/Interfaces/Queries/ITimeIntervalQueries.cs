using BookRight.Facade.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookRight.Facade.Interfaces.Queries
{
    public interface ITimeIntervalQueries
    {
        Task<TimeIntervalDto?> GetAsync(Guid id);
        Task<IReadOnlyList<TimeIntervalDto>> GetAllAsync();
        
    }
}
