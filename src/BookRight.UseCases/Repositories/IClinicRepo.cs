using BookRight.Domain.Entities;

namespace BookRight.UseCases.Repositories;

public interface IClinicRepo
{
    Task AddClinicAsync(Clinic clinic);
    Task<Clinic?> GetClinic_ByIdAsync(Guid id);
}