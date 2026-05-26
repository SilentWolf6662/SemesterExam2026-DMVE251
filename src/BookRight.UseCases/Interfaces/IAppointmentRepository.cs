using BookRight.Domain.Entities;

namespace BookRight.UseCases.Repositories;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Appointment>> GetAllByPractitionerIdAsync(Guid practitionerId);
    Task<IReadOnlyList<Appointment>> GetAllByPatientIdAsync(Guid patientId);
    Task<IReadOnlyList<Appointment>> GetAllByClinicIdAsync(Guid clinicId);
    Task<decimal> GetSumOf12MonthsByPatientIdAsync(Guid patientId, DateTime startDate);
    Task<int> GetBirthdayDiscountUsedCountByPatientIdAsync(Guid patientId, DateTime startDate);
    Task AddAsync(Appointment appointment);
    Task SaveAsync();
}