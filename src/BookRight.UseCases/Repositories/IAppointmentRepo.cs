using BookRight.Domain.Entities;

namespace BookRight.UseCases.Repositories;

public interface IAppointmentRepo
{
    Task<Appointment?> GetAppointment_ByIdAsync(Guid id);
    Task<IReadOnlyList<Appointment>> GetAppointments_ByPractitionerIdAsync(Guid practitionerId);
    Task<IReadOnlyList<Appointment>> GetAppointments_ByPatientIdAsync(Guid patientId);
    Task AddAppointmentAsync(Appointment appointment);
    //Task UpdateAppointmentStatus_ToCompletedAsync(Appointment appointment);
    //Task UpdateAppointmentStatus_ToCancelledAsync(Appointment appointment);
    //Task UpdateAppointmentStatus_ToNoShowAsync(Appointment appointment);
}