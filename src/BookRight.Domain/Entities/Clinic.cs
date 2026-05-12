using BookRight.Domain.ValueObjects;

namespace BookRight.Domain.Entities;

public class Clinic : AggregateRoot
{
    public Address ClinicAddress { get; private set; } = null!;
    public List<TimeInterval> WorkingHours { get; private set; } = null!;
    public int Rooms { get; private set; }

    // PRIVAT constructor — tvinger brug af factory-metoden Create()
    private Clinic() { } // EF Core
    private Clinic(Address clinicAddress, List<TimeInterval> workingHours, int rooms)
    {
        ClinicAddress = clinicAddress;
        WorkingHours = workingHours;
        Rooms = rooms;
    }

    // ── Factory-metode: eneste måde at oprette en klinik ──────
    public static Clinic Create(Address clinicAddress, List<TimeInterval> workingHours, int rooms)
    {
        // Laver og retuner en ny clinic med en adresse, åbningstider og antal rum for klinikken
        return new Clinic(clinicAddress, workingHours, rooms);
    }
}