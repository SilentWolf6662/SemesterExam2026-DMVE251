using BookRight.Domain.Entities;
using BookRight.Domain.ValueObjects;

namespace BookRight.Domain.Tests.Entities;

// Tests for Clinic-entiteten (aggregat-root).

// Dækker:
//   - Oprettelse via factory-metoden Create()
//   - At adresse, åbningstider og antal rum sættes korrekt
public class ClinicTests
{
    // ── Hjælpe-data ───────────────────────────────────────────────────────────

    private static readonly Address TestAddress = new("Klinikgade 5", 8000);

    // Opretter en simpel liste med ét åbningstids-interval (08:00-16:00).
    private static List<TimeInterval> CreateOpeningHours() =>
    [
        new(new DateTime(2026, 1, 1, 8, 0, 0), new DateTime(2026, 1, 1, 16, 0, 0))
    ];

    // Opretter en Clinic med standardværdier der kan overskrives pr. parameter.
    private static Clinic CreateClinic(int rooms = 3, List<TimeInterval>? openingHours = null, Address? address = null) =>
        Clinic.Create(address ?? TestAddress, openingHours ?? CreateOpeningHours(), rooms);

    // ── Oprettelse ────────────────────────────────────────────────────────────

    // Tester at factory-metoden returnerer et Clinic-objekt og ikke null.
    [Fact]
    public void Create_Clinic_WithValidValues_ReturnsNotNull()
    {
        // Arrange & Act
        var clinic = CreateClinic();

        // Assert
        Assert.NotNull(clinic);
    }

    // Tester at Rooms-feltet matcher det antal rum der gives til Create().
    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void Create_Clinic_HasCorrectNumberOfRooms(int roomCount)
    {
        // Arrange & Act
        var clinic = CreateClinic(roomCount);

        // Assert
        Assert.Equal(roomCount, clinic.Rooms);
    }

    // Test Data for Create_Clinic_HasCorrectAddress
    public static IEnumerable<Address[]> GetAddresses()
    {
        yield return [new Address("Klinikgade 10", 8100)];
        yield return [new Address("Klinikgade 15", 8100)];
        yield return [TestAddress];
    }

    // Tester at ClinicAddress peger på den adresse der gives til Create().
    [Theory]
    [MemberData(nameof(GetAddresses))]
    public void Create_Clinic_HasCorrectAddress(Address address)
    {
        // Arrange & Act
        var clinic = CreateClinic(address: address);

        // Assert
        Assert.Equal(address, clinic.ClinicAddress);
    }

    // Test Data for Create_Clinic_HasOpeningHours
    public static IEnumerable<object[]> GetOpeningHours()
    {
        yield return new object[]
        {
            new List<TimeInterval>
            {
                new(new DateTime(2026, 1, 1, 8, 0, 0), new DateTime(2026, 1, 1, 16, 0, 0)),

                new(new DateTime(2026, 1, 1, 8, 0, 0), new DateTime(2026, 1, 1, 16, 0, 0))
            }
        };
        yield return new object[]
        {
            new List<TimeInterval>
            {
                new(new DateTime(2026, 1, 1, 8, 0, 0), new DateTime(2026, 1, 1, 10, 0, 0)),

                new(new DateTime(2026, 1, 1, 13, 0, 0), new DateTime(2026, 1, 1, 16, 0, 0)),

                new(new DateTime(2026, 1, 1, 8, 0, 0), new DateTime(2026, 1, 1, 16, 0, 0))
            }
        };
    }

    // Tester at WorkingHours indeholder de tidsintervaller der gives til Create().
    [Theory]
    [MemberData(nameof(GetOpeningHours))]
    public void Create_Clinic_HasOpeningHours(List<TimeInterval> openingHours)
    {
        // Arrange & Act
        var clinic = CreateClinic(openingHours: openingHours);

        // Assert
        Assert.Equal(openingHours, clinic.WorkingHours);
    }

    // Tester at WorkingHours-listen for to separate klinikker ikke peger på det samme list objekt.
    [Fact]
    public void Create_TwoClinics_DoNotShareOpeningHoursListReference()
    {
        // Arrange
        var time = new List<TimeInterval>
        {
            new(new DateTime(2026, 1, 1, 8, 0, 0), new DateTime(2026, 1, 1, 16, 0, 0))
        };

        var time2 = new List<TimeInterval>
        {
            new(new DateTime(2026, 2, 1, 8, 0, 0), new DateTime(2026, 2, 1, 16, 0, 0))
        };

        // Act
        var clinic = CreateClinic(openingHours: time);
        var clinic2 = CreateClinic(openingHours: time2, address: new Address("Klinikgade 15", 8100));

        // Assert
        Assert.NotEqual(clinic.WorkingHours, clinic2.WorkingHours);
    }
}
