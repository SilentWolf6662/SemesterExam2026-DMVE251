using BookRight.Domain.Enums;
using BookRight.Facade.DTO;
using BookRight.Facade.Interfaces.Queries;
using Microsoft.EntityFrameworkCore;

namespace BookRight.Infrastructure.Query;

public class ClinicOccupancyQueriesImpl : IClinicOccupancyQueries
{
    private readonly AppDbContext _db;

    public ClinicOccupancyQueriesImpl(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ClinicOccupancyDto>> GetTodayAsync()
    {
        var dayStart = DateTime.Today;
        var dayEnd = dayStart.AddDays(1);

        var clinics = await _db.Clinics.AsNoTracking().ToListAsync();

        // Hent dagens aktive aftaler (ikke aflyste) — hentes i hukommelsen for at kunne beregne peak
        var todayAppointments = await _db.Appointments
            .AsNoTracking()
            .Where(a => a.TimeInterval.Start >= dayStart
                     && a.TimeInterval.Start < dayEnd
                     && a.Status != AppointmentStatus.Cancelled)
            .Select(a => new { a.ClinicId, Start = a.TimeInterval.Start, End = a.TimeInterval.End })
            .ToListAsync();

        return clinics.Select(c =>
        {
            var clinicApts = todayAppointments.Where(a => a.ClinicId == c.Id).ToList();

            // Peak samtidige rum = det højeste antal overlappende aftaler på et givet tidspunkt
            int peak = 0;
            foreach (var apt in clinicApts)
            {
                int concurrent = clinicApts.Count(other => other.Start < apt.End && other.End > apt.Start);
                if (concurrent > peak) peak = concurrent;
            }

            double pct = c.Rooms > 0 ? Math.Min((double)peak / c.Rooms * 100, 100) : 0;

            return new ClinicOccupancyDto(
                c.Id,
                $"{c.ClinicAddress.StreetName}, {c.ClinicAddress.Zipcode}",
                c.Rooms,
                clinicApts.Count,
                peak,
                Math.Round(pct, 1));
        }).ToList();
    }
}
