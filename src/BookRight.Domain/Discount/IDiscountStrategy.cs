using BookRight.Domain.Entities;
using BookRight.Domain.ValueObjects;

namespace BookRight.Domain.Discount;

public interface IDiscountStrategy
{
    string Name { get; }
    CalculatedDiscount Calculate(Appointment appointment, TimeInterval timeInterval);
}