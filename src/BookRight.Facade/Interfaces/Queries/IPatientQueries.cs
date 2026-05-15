using System;
using System.Collections.Generic;
using System.Text;
using BookRight.Facade.DTO;

namespace BookRight.Facade.Interfaces.Queries
{
    public interface IPatientQueries
    {
        Task<PatientDetailedDto?> GetAsync(Guid id);
        Task<IReadOnlyList<PatientDto>> GetAllAsync();
    }
}
