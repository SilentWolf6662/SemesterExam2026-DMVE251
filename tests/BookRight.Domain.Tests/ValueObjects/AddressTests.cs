namespace BookRight.Domain.Tests.ValueObjects;

/// <summary>
/// Tests for Address-value-objektet.
///
/// Dækker:
///   - Oprettelse via den offentlige konstruktør (gadenavn + postnummer)
///   - At StreetName og Zipcode gemmes korrekt
///   - Record value-equality: to instanser med samme data er ens
///   - Record reference-semantik: to instanser er separate objekter
///
///</summary>
public class AddressTests
{
    // ── Oprettelse og feltværdier ─────────────────────────────────────────────

    // Tester at konstruktøren returnerer et Address-objekt og ikke null.
    [Fact]
    public void Create_Address_ReturnsNotNull()
    {
    }

    // Tester at StreetName matcher det gadenavn der gives til konstruktøren.
    [Fact]
    public void Create_Address_HasCorrectStreetName()
    {
    }

    // Tester at Zipcode matcher det postnummer der gives til konstruktøren.
    [Fact]
    public void Create_Address_HasCorrectZipcode()
    {
    }

    // Tester at både gadenavn og postnummer sættes korrekt i samme oprettelse.
    [Fact]
    public void Create_Address_BothValuesAreStoredCorrectly()
    {
    }

    // ── Record value-equality ─────────────────────────────────────────────────

    // Tester at to Address-instanser med identiske feltværdier er value-equal (record-semantik).
    [Fact]
    public void Address_WithSameValues_AreEqual()
    {
    }

    // Tester at forskelligt gadenavn medfører at de to adresser ikke er ens.
    [Fact]
    public void Address_WithDifferentStreetName_AreNotEqual()
    {
    }

    // Tester at forskelligt postnummer medfører at de to adresser ikke er ens.
    [Fact]
    public void Address_WithDifferentZipcode_AreNotEqual()
    {
    }

    // Tester at to value-equal adresser stadig er separate objekter i hukommelsen.
    [Fact]
    public void Address_WithSameValues_AreNotSameReference()
    {
    }
}
