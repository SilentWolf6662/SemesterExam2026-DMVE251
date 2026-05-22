using BookRight.Domain.Entities;

namespace BookRight.Domain.Discount;

public interface IDiscountStrategy
{
    string Name { get; }
    Task<CalculatedDiscount> Calculate(decimal currentPrice, Appointment appointment);
}