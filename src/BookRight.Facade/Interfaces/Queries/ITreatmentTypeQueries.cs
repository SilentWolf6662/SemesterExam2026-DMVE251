using BookRight.Facade.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookRight.Facade.Interfaces.Queries
{
    public interface ITreatmentTypeQueries
    {
        Task<TreatmentTypeDto?> GetAsync(Guid id);
        Task<IReadOnlyList<TreatmentTypeDto>> GetAllAsync();
        
    }
}
