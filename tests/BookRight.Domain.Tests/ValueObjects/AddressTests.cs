using BookRight.Domain.ValueObjects;

namespace BookRight.Domain.Tests.ValueObjects;

/// <summary>
/// Tests for Address-value-objektet.
///
/// Dækker:
/// - Oprettelse af Address
/// - At StreetName og Zipcode gemmes korrekt
/// - Record value equality
/// - Record reference-semantik
/// </summary>
public class AddressTests
{
    // ── Oprettelse og feltværdier ─────────────────────────────────────────────

    /// <summary>
    /// Verificerer at Address gemmer StreetName og Zipcode korrekt
    /// ved oprettelse via konstruktøren.
    /// </summary>
    /// <param name="streetName">Gadenavn der anvendes ved oprettelse.</param>
    /// <param name="zipcode">Postnummer der anvendes ved oprettelse.</param>
    [Theory]
    [InlineData("Testvej 1", 1234)]
    [InlineData("Nørrebrogade 42", 2200)]
    [InlineData("Vestergade 10", 5000)]
    [InlineData("Østergade 7", 1100)]
    public void Constructor_ValidValues_SetsPropertiesCorrectly(string streetName, int zipcode)
    {
        // Arrange & Act
        var address = new Address(streetName, zipcode);

        // Assert
        Assert.NotNull(address);
        Assert.Equal(streetName, address.StreetName);
        Assert.Equal(zipcode, address.Zipcode);
    }

    // ── Record value equality ─────────────────────────────────────────────────

    /// <summary>
    /// Verificerer at to Address-instancer med identiske værdier
    /// er value-equal.
    /// </summary>
    /// <param name="street1">Første adressens gadenavn.</param>
    /// <param name="zip1">Første adressens postnummer.</param>
    /// <param name="street2">Anden adressens gadenavn.</param>
    /// <param name="zip2">Anden adressens postnummer.</param>
    [Theory]
    [InlineData("Testvej 1", 1234, "Testvej 1", 1234)]
    [InlineData("Nørrebrogade 42", 2200, "Nørrebrogade 42", 2200)]
    public void Equals_SameValues_ReturnsTrue(string street1, int zip1, string street2, int zip2)
    {
        // Arrange
        var address1 = new Address(street1, zip1);
        var address2 = new Address(street2, zip2);

        // Act & Assert
        Assert.Equal(address1, address2);
    }

    /// <summary>
    /// Verificerer at to Address-instancer med forskellige værdier
    /// ikke er value-equal.
    /// </summary>
    /// <param name="street1">Første adressens gadenavn.</param>
    /// <param name="zip1">Første adressens postnummer.</param>
    /// <param name="street2">Anden adressens gadenavn.</param>
    /// <param name="zip2">Anden adressens postnummer.</param>
    [Theory]
    [InlineData("Testvej 1", 1234, "Andenvej 2", 1234)]
    [InlineData("Testvej 1", 1234, "Testvej 1", 5678)]
    public void Equals_DifferentValues_ReturnsFalse(string street1, int zip1, string street2, int zip2)
    {
        // Arrange
        var address1 = new Address(street1, zip1);
        var address2 = new Address(street2, zip2);

        // Act & Assert
        Assert.NotEqual(address1, address2);
    }

    // ── Reference-semantik ────────────────────────────────────────────────────

    /// <summary>
    /// Verificerer at to Address-instancer med identiske værdier
    /// ikke refererer til samme objekt i hukommelsen.
    /// </summary>
    [Fact]
    public void Constructor_SameValues_CreatesDifferentReferences()
    {
        // Arrange & Act
        var address1 = new Address("Testvej 1", 1234);
        var address2 = new Address("Testvej 1", 1234);

        // Assert
        Assert.False(ReferenceEquals(address1, address2));
    }
}