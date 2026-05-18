using BookRight.Domain.Entities;

namespace BookRight.UseCases.Repositories;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Appointment>> GetAllByPractitionerIdAsync(Guid practitionerId);
    Task<IReadOnlyList<Appointment>> GetAllByPatientIdAsync(Guid patientId);
    Task AddAsync(Appointment appointment);
    Task SaveAsync();
}