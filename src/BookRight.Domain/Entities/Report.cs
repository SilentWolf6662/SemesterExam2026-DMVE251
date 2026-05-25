using System;
using System.Collections.Generic;
using System.Text;
using BookRight.Domain.ValueObjects;

namespace BookRight.Domain.Entities
{
    public class Report
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public DateTime GeneratedDate { get; private set; }
        public DateOnly PeriodStart { get; private set; }
        public DateOnly PeriodEnd { get; private set; }
        public decimal TotalRevenue { get; private set; }
        

        private Report() {}

        public static Report Create(DateOnly periodStart, DateOnly periodEnd, decimal totalRevenue )
        {
            return new Report
            {
                Id = Guid.NewGuid(),
                Title = "Omsætningsrapport",
                GeneratedDate = DateTime.Now,
                PeriodStart = periodStart,
                PeriodEnd = periodEnd,
                TotalRevenue = totalRevenue
            };
        }
    }
}
