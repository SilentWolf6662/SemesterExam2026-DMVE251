using System.Reflection.Metadata.Ecma335;
using BookRight.Domain.Entities;
using BookRight.Facade.DTO;
using BookRight.Facade.Interfaces.Queries;
using Microsoft.EntityFrameworkCore;

namespace BookRight.Infrastructure.Query
{
    public class ReportQueriesImpl : IReportQueries
    {
        private readonly AppDbContext _db;

        public ReportQueriesImpl(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ReportDto> GenerateAsync(DateOnly periodStart, DateOnly periodEnd)
        {
            var totalRevenue = await _db.Appointments
                .Where(a => DateOnly.FromDateTime(a.TimeInterval.Start) >= periodStart && DateOnly.FromDateTime(a.TimeInterval.Start) <= periodEnd)
                .SumAsync(a => a.Price);
            return new ReportDto(Guid.NewGuid(), "Omsætningsrapport", DateTime.Now, periodStart, periodEnd, totalRevenue);
        }
    }
}
