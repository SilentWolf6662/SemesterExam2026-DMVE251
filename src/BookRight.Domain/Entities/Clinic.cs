using BookRight.Domain.ValueObjects;

namespace BookRight.Domain.Entities;

public class Clinic : AggregateRoot
{
    public Address ClinicAddress { get; private set; } = null!;
    public WorkingHour WorkingHours { get; private set; } = null!;
    public int Rooms { get; private set; }
    private Clinic() { }
    private Clinic(Address clinicAddress, WorkingHour workingHours, int rooms)
    {
        ClinicAddress = clinicAddress;
        WorkingHours = workingHours;
        Rooms = rooms;
    }
    public static Clinic Create(Address clinicAddress, WorkingHour workingHours, int rooms)
    {
        return new Clinic(clinicAddress, workingHours, rooms);
    }
}