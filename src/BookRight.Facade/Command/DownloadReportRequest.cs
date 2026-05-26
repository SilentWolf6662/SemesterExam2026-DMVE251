namespace BookRight.Facade.Command;

public record DownloadReportRequest(
    DateTime GeneratedDate,
    DateOnly PeriodStart,
    DateOnly PeriodEnd,
    decimal EstimatedRevenue,
    decimal TotalRevenue);