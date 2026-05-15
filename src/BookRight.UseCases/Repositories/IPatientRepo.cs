using BookRight.Domain.Entities;

namespace BookRight.UseCases.Repositories;

public interface IPatientRepo
{
    Task AddPatientAsync(Patient patient);
    Task<Patient?> GetPatient_ByIdAsync(Guid id);
}