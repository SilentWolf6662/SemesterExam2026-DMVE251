using BookRight.Facade.DTO;

namespace BookRight.Facade.Interfaces.Queries
{
    public interface IReportQueries
    {
        Task<ReportDto> GenerateAsync(DateOnly periodStart, DateOnly periodEnd);
    }
}
