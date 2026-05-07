using BookRight.Domain.ValueObjects;

namespace BookRight.Domain.Entities
{
    public class Clinic : AggregateRoot
    {
        public Address ClinicAddress { get; private set; } = null!;
        public WorkingHour WorkingHours { get; private set; } = null!;
        public int Rooms { get; private set; }
    }
}