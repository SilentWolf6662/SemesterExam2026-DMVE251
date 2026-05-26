using System;
using System.Collections.Generic;
using System.Text;

namespace BookRight.Facade.Command
{
    public record DownloadReportRequest(
        DateTime GeneratedDate,
        DateOnly PeriodStart,
        DateOnly PeriodEnd,
        decimal EstimatedRevenue,
        decimal TotalRevenue);
}
