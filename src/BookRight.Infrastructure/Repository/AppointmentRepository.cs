using BookRight.Domain.Entities;
using BookRight.Domain.Enums;
using BookRight.UseCases.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookRight.Infrastructure.Repository;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppDbContext _db;

    public AppointmentRepository(AppDbContext db)
    {
        _db = db;
    }

    // Henter en appointment baseret på dens Id
    async Task<Appointment?> IAppointmentRepository.GetByIdAsync(Guid id)
    {
        return await _db.Appointments
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    // Henter alle appointments baseret på practitioner (behandler) Id
    async Task<IReadOnlyList<Appointment>> IAppointmentRepository.GetAllByPractitionerIdAsync(Guid practitionerId)
    {
        return await _db.Appointments
            .AsNoTracking()
            .Where(a => a.PractitionerId == practitionerId)
            .ToListAsync();
    }

    // Henter alle appointments baseret på patient Id
    async Task<IReadOnlyList<Appointment>> IAppointmentRepository.GetAllByPatientIdAsync(Guid patientId)
    {
        return await _db.Appointments
            .AsNoTracking()
            .Where(a => a.PatientId == patientId)
            .ToListAsync();
    }

    // Henter summen for alle patientes appointments de sidste 12 måneder
    async Task<decimal> IAppointmentRepository.GetSumOf12MonthsByPatientIdAsync(Guid patientId, DateTime startDate)
    {
        var twelveMonthsAgo = startDate.AddMonths(-11);
        return await _db.Appointments
            .Where(a => a.PatientId == patientId &&
                        a.TimeInterval.Start >= twelveMonthsAgo && // Tidsrum over for 12 måneder siden
                        a.TimeInterval.Start <= startDate && // og før den aktuelle booking
                        a.Status == AppointmentStatus.Completed) // Kun fuldførte bookinger tælles med
            .SumAsync(a => a.Price); // Lav en sum af alle følgende bookinger
    }

    // Henter alle patientens appointments hvor der er brugt fødselsdagsrabat de sidste 12 måneder
    async Task<int> IAppointmentRepository.GetBirthdayDiscountUsedCountByPatientIdAsync(Guid patientId, DateTime startDate)
    {
        var twelveMonthsAgo = startDate.AddMonths(-11);
        return await _db.Appointments
            .Where(a => a.PatientId == patientId &&
                        a.TimeInterval.Start >= twelveMonthsAgo && // Tidsrum over for 12 måneder siden
                        a.TimeInterval.Start <= startDate && // og før den aktuelle booking
                        a.Status == AppointmentStatus.Completed && // Kun fuldførte bookinger tælles med
                        a.DiscountType == DiscountType.Birthday) // Kun bookinger hvor fødselsdagsrabatten er brugt tælles med
            .CountAsync(); // Tæl antallet af bruge fødselsdagsrabatter de sidste 12 måneder
    }

    // Tilføjer en ny appointment til databasen
    Task IAppointmentRepository.SaveAsync()
    {
        return _db
            .SaveChangesAsync();
    }

    // Gemmer i database
    async Task IAppointmentRepository.AddAsync(Appointment appointment)
    {
        await _db.Appointments
            .AddAsync(appointment);
    }
}