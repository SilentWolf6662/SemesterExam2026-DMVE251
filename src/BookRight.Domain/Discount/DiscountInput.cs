namespace BookRight.UseCases.Discount;

public record DiscountInput(
    decimal CurrentPrice,
    DateTime To,
    DateTime From,
    DateOnly PatientBirthDate,
    decimal PatientBooking12MonthTotalSum,
    int BirthdayDiscountUsedCount,
    decimal CampaignDiscountRate,
    string CampaignName
);
