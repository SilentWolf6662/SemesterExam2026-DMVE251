namespace BookRight.Facade.DTO
{
    public record ReportDto
    (
        Guid Id,
        string Title,
        DateTime GeneratedDate,
        DateOnly PeriodStart,
        DateOnly PeriodEnd,
        decimal EstimatedRevenue,
        decimal TotalRevenue
    );
}
