using BookRight.Facade.DTO;
using BookRight.Facade.Interfaces.Queries;
using Microsoft.EntityFrameworkCore;

namespace BookRight.Infrastructure.Query;

public class AppointmentQueriesImpl : IAppointmentQueries
{
    private readonly AppDbContext _db;

    public AppointmentQueriesImpl(AppDbContext db)
    {
        _db = db;
    }

    //Henter alle aftaler fra databasen og returnerer dem som en read-only liste af DTO'er.
    // Kombinerede aftaler (samme CombinedBookingId) flettes til ét opslag der spænder hele perioden.
    async Task<IReadOnlyList<AppointmentDto>> IAppointmentQueries.GetAllAsync()
    {
        var raw = await _db.Appointments.AsNoTracking()
            .Select(a => new
            {
                a.Id,
                a.TimeInterval.Start,
                a.TimeInterval.End,
                a.TreatmentTypeId,
                a.PractitionerId,
                a.Status,
                a.Price,
                a.CombinedBookingId
            })
            .ToListAsync();

        return raw
            .GroupBy(a => a.CombinedBookingId ?? a.Id)
            .Select(g =>
            {
                var first = g.MinBy(a => a.Start)!;
                var last  = g.MaxBy(a => a.Start)!;
                return new AppointmentDto(first.Id, first.Start, last.End, first.TreatmentTypeId, first.PractitionerId, first.Status.ToString(), g.Sum(a => a.Price), first.CombinedBookingId);
            })
            .ToList();
    }

    // Henter en enkelt aftale med detaljerede oplysninger baseret på id
    // Returnerer null hvis aftalen ikke findes
    async Task<AppointmentDetailedDto?> IAppointmentQueries.GetAsync(Guid id)
    {
        return await _db.Appointments.AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => new AppointmentDetailedDto(
                a.Id, a.TimeInterval.Start, a.TimeInterval.End,
                a.TreatmentTypeId, a.PatientId, a.PractitionerId,
                a.Status.ToString(), a.Note, a.Price, a.CombinedBookingId))
            .FirstOrDefaultAsync();
    }

    // Henter begge aftaler i en kombineret booking, sorteret efter starttidspunkt
    async Task<IReadOnlyList<AppointmentDetailedDto>> IAppointmentQueries.GetAllByCombinedBookingIdAsync(Guid combinedBookingId)
    {
        var results = await _db.Appointments.AsNoTracking()
            .Where(a => a.CombinedBookingId == combinedBookingId)
            .Select(a => new AppointmentDetailedDto(
                a.Id, a.TimeInterval.Start, a.TimeInterval.End,
                a.TreatmentTypeId, a.PatientId, a.PractitionerId,
                a.Status.ToString(), a.Note, a.Price, a.CombinedBookingId))
            .ToListAsync();
        return [.. results.OrderBy(a => a.Start)];
    }
}
