using BookRight.Facade.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookRight.Facade.Interfaces.Queries
{
    public interface IPatientDetailedQueries
    {
        Task<PatientDetailedDto?> GetAsync(Guid id);
        Task<IReadOnlyList<PatientDetailedDto>> GetAllAsync();
    }
}
