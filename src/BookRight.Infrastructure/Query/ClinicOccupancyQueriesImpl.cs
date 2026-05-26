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

        // Henter alle klinikker — vi skal bruge Rooms til at beregne belægningsprocenten
        var clinics = await _db.Clinics.AsNoTracking().ToListAsync();

        // Henter dagens ikke-aflyste aftaler som anonyme objekter i hukommelsen.
        // Vi henter kun de felter vi rent faktisk bruger (ClinicId, Start, End)
        // for at holde dataoverførslen minimal.
        // Aflyste aftaler tæller ikke som rumforbrug.
        var todayAppointments = await _db.Appointments
            .AsNoTracking()
            .Where(a => a.TimeInterval.Start >= dayStart
                     && a.TimeInterval.Start < dayEnd
                     && a.Status != AppointmentStatus.Cancelled)
            .Select(a => new { a.ClinicId, Start = a.TimeInterval.Start, End = a.TimeInterval.End })
            .ToListAsync();

        return clinics.Select(c =>
        {
            // Filtrer til kun denne kliniKs aftaler
            var clinicApts = todayAppointments.Where(a => a.ClinicId == c.Id).ToList();

            // Peak samtidige rum beregnes ved at tælle for hver aftale hvor mange andre
            // aftaler overlapper med den. Overlapping er defineret som: anden starter
            // før denne slutter OG anden slutter efter denne starter.
            // Det højeste antal samtidige aftaler er peak-belægningen.
            int peak = 0;
            foreach (var apt in clinicApts)
            {
                int concurrent = clinicApts.Count(other => other.Start < apt.End && other.End > apt.Start);
                if (concurrent > peak) peak = concurrent;
            }

            // Belægningsprocenten afrundes til én decimal og begrænses til 100 %
            // i det usandsynlige tilfælde at peak overstiger det registrerede rumantal
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
