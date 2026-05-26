using BookRight.Domain.Discount.DiscountStrategy;
using BookRight.Domain.Enums;
using BookRight.UseCases.Discount;
using BookRight.UseCases.Discount.DiscountStrategy;

namespace BookRight.UseCases.Tests.Discounts;

// Tests for alle fem rabatstrategier.
//
// Dækker:
//   - Bronze/Sølv/Guld loyalitetsrabat: korrekt tærskel, rabatsats og IsApplicable-flag
//   - Fødselsdagsrabat: fødselsmåned-match, maks. én gang pr. 12 måneder
//   - Black Friday-rabat: præcis dato og ikke-matchende datoer

public class DiscountStrategyTests
{
    // Opretter et DiscountInput med mulighed for at overskrive enkeltfelter.
    private static DiscountInput CreateInput(
        decimal currentPrice = 500m,
        DateOnly? birthDate = null,
        decimal monthlySum = 0m,
        int birthdayUsedCount = 0,
        DateTime? from = null)
    {
        var start = from ?? new DateTime(2026, 6, 15, 10, 0, 0);
        return new DiscountInput(
            currentPrice,
            start.AddHours(1),
            start,
            birthDate ?? new DateOnly(1990, 3, 15),
            monthlySum,
            birthdayUsedCount);
    }

    // ── Bronze Loyalitetsrabat ─────────────────────────────────────────────────

    // Tester at 5 % rabat gives når omsætning er inden for Bronze-intervallet (3.000–10.000 kr).
    [Theory]
    [InlineData(3000)]
    [InlineData(6500)]
    [InlineData(10000)]
    public async Task BronzeStrategy_SumInRange_Returns5PercentDiscount(decimal sum)
    {
        // Arrange
        IDiscountStrategy strategy = new BronzeLoyaltyDiscountStrategy();
        var input = CreateInput(currentPrice: 1000m, monthlySum: sum);

        // Act
        var result = await strategy.Calculate(input);

        // Assert
        Assert.True(result.IsApplicable);
        Assert.Equal(50m, result.DiscountAmount); // 5 % af 1.000 kr = 50 kr
        Assert.Equal(DiscountType.BronzeLoyalty, result.DiscountType);
    }

    // Tester at ingen rabat gives når omsætning er under Bronze-minimumsgrænsen (3.000 kr).
    [Theory]
    [InlineData(0)]
    [InlineData(2999)]
    public async Task BronzeStrategy_SumBelowRange_ReturnsNoDiscount(decimal sum)
    {
        // Arrange
        IDiscountStrategy strategy = new BronzeLoyaltyDiscountStrategy();
        var input = CreateInput(monthlySum: sum);

        // Act
        var result = await strategy.Calculate(input);

        // Assert
        Assert.False(result.IsApplicable);
        Assert.Equal(0m, result.DiscountAmount);
    }

    // Tester at ingen rabat gives når omsætning overstiger Bronze-maksimumgrænsen (10.000 kr).
    [Theory]
    [InlineData(10001)]
    [InlineData(25000)]
    public async Task BronzeStrategy_SumAboveRange_ReturnsNoDiscount(decimal sum)
    {
        // Arrange
        IDiscountStrategy strategy = new BronzeLoyaltyDiscountStrategy();
        var input = CreateInput(monthlySum: sum);

        // Act
        var result = await strategy.Calculate(input);

        // Assert
        Assert.False(result.IsApplicable);
        Assert.Equal(0m, result.DiscountAmount);
    }

    // ── Sølv Loyalitetsrabat ──────────────────────────────────────────────────

    // Tester at 10 % rabat gives når omsætning er inden for Sølv-intervallet (10.001–25.000 kr).
    [Theory]
    [InlineData(10001)]
    [InlineData(17500)]
    [InlineData(25000)]
    public async Task SilverStrategy_SumInRange_Returns10PercentDiscount(decimal sum)
    {
        // Arrange
        IDiscountStrategy strategy = new SilverLoyaltyDiscountStrategy();
        var input = CreateInput(currentPrice: 1000m, monthlySum: sum);

        // Act
        var result = await strategy.Calculate(input);

        // Assert
        Assert.True(result.IsApplicable);
        Assert.Equal(100m, result.DiscountAmount); // 10 % af 1.000 kr = 100 kr
        Assert.Equal(DiscountType.SilverLoyalty, result.DiscountType);
    }

    // Tester at ingen rabat gives for omsætninger der hører til Bronze- eller Guld-intervallet.
    [Theory]
    [InlineData(0)]
    [InlineData(10000)]  // Øvre grænse for Bronze
    [InlineData(25001)]  // Nedre grænse for Guld
    public async Task SilverStrategy_SumOutsideRange_ReturnsNoDiscount(decimal sum)
    {
        // Arrange
        IDiscountStrategy strategy = new SilverLoyaltyDiscountStrategy();
        var input = CreateInput(monthlySum: sum);

        // Act
        var result = await strategy.Calculate(input);

        // Assert
        Assert.False(result.IsApplicable);
        Assert.Equal(0m, result.DiscountAmount);
    }

    // ── Guld Loyalitetsrabat ──────────────────────────────────────────────────

    // Tester at 15 % rabat gives når omsætning overstiger Guld-grænsen (25.000 kr).
    [Theory]
    [InlineData(25001)]
    [InlineData(50000)]
    public async Task GoldStrategy_SumAboveThreshold_Returns15PercentDiscount(decimal sum)
    {
        // Arrange
        IDiscountStrategy strategy = new GoldLoyaltyDiscountStrategy();
        var input = CreateInput(currentPrice: 1000m, monthlySum: sum);

        // Act
        var result = await strategy.Calculate(input);

        // Assert
        Assert.True(result.IsApplicable);
        Assert.Equal(150m, result.DiscountAmount); // 15 % af 1.000 kr = 150 kr
        Assert.Equal(DiscountType.GoldLoyalty, result.DiscountType);
    }

    // Tester at ingen rabat gives for omsætninger på eller under 25.000 kr.
    [Theory]
    [InlineData(0)]
    [InlineData(24999)]
    [InlineData(25000)]
    public async Task GoldStrategy_SumAtOrBelowThreshold_ReturnsNoDiscount(decimal sum)
    {
        // Arrange
        IDiscountStrategy strategy = new GoldLoyaltyDiscountStrategy();
        var input = CreateInput(monthlySum: sum);

        // Act
        var result = await strategy.Calculate(input);

        // Assert
        Assert.False(result.IsApplicable);
        Assert.Equal(0m, result.DiscountAmount);
    }

    // ── Fødselsdagsrabat ─────────────────────────────────────────────────────

    // Tester at 25 % rabat gives når patientens fødselsmåned matcher den aktuelle måned
    // og fødselsdagsrabatten ikke er brugt endnu i indeværende år.
    [Fact]
    public async Task BirthdayStrategy_MatchingMonthAndUnused_Returns25PercentDiscount()
    {
        // Arrange
        IDiscountStrategy strategy = new BirthdayDiscountStrategy();
        var birthDate = new DateOnly(1990, DateTime.Now.Month, 15); // Fødselsmåned = aktuel måned
        var input = CreateInput(currentPrice: 1000m, birthDate: birthDate, birthdayUsedCount: 0);

        // Act
        var result = await strategy.Calculate(input);

        // Assert
        Assert.True(result.IsApplicable);
        Assert.Equal(250m, result.DiscountAmount); // 25 % af 1.000 kr = 250 kr
        Assert.Equal(DiscountType.Birthday, result.DiscountType);
    }

    // Tester at ingen rabat gives når patientens fødselsmåned ikke matcher den aktuelle måned.
    [Fact]
    public async Task BirthdayStrategy_NonMatchingMonth_ReturnsNoDiscount()
    {
        // Arrange
        IDiscountStrategy strategy = new BirthdayDiscountStrategy();
        // Vælg en fødselsmåned der aldrig matcher den aktuelle måned
        var otherMonth = (DateTime.Now.Month % 12) + 1;
        var birthDate = new DateOnly(1990, otherMonth, 15);
        var input = CreateInput(birthDate: birthDate);

        // Act
        var result = await strategy.Calculate(input);

        // Assert
        Assert.False(result.IsApplicable);
        Assert.Equal(0m, result.DiscountAmount);
    }

    // Tester at ingen rabat gives når fødselsdagsrabatten allerede er brugt én gang
    // i de seneste 12 måneder, selv om fødselsmåneden matcher.
    [Fact]
    public async Task BirthdayStrategy_MatchingMonthButAlreadyUsed_ReturnsNoDiscount()
    {
        // Arrange
        IDiscountStrategy strategy = new BirthdayDiscountStrategy();
        var birthDate = new DateOnly(1990, DateTime.Now.Month, 15);
        var input = CreateInput(birthDate: birthDate, birthdayUsedCount: 1); // Rabatten er allerede brugt

        // Act
        var result = await strategy.Calculate(input);

        // Assert
        Assert.False(result.IsApplicable);
        Assert.Equal(0m, result.DiscountAmount);
    }

    // ── Black Friday-rabat ────────────────────────────────────────────────────

    // Black Friday 2026 = 27. november (den 4. fredag i november).
    private static readonly DateTime BlackFriday2026 = new DateTime(2026, 11, 27, 10, 0, 0);

    // Tester at 25 % rabat gives præcis på Black Friday.
    [Fact]
    public async Task BlackFridayStrategy_OnBlackFriday_Returns25PercentDiscount()
    {
        // Arrange
        IDiscountStrategy strategy = new BlackFridayDiscountStrategy();
        var input = CreateInput(currentPrice: 1000m, from: BlackFriday2026);

        // Act
        var result = await strategy.Calculate(input);

        // Assert
        Assert.True(result.IsApplicable);
        Assert.Equal(250m, result.DiscountAmount); // 25 % af 1.000 kr = 250 kr
        Assert.Equal(DiscountType.BlackFriday, result.DiscountType);
    }

    // Tester at ingen rabat gives dagen før Black Friday.
    [Fact]
    public async Task BlackFridayStrategy_DayBeforeBlackFriday_ReturnsNoDiscount()
    {
        // Arrange
        IDiscountStrategy strategy = new BlackFridayDiscountStrategy();
        var input = CreateInput(from: BlackFriday2026.AddDays(-1));

        // Act
        var result = await strategy.Calculate(input);

        // Assert
        Assert.False(result.IsApplicable);
        Assert.Equal(0m, result.DiscountAmount);
    }

    // Tester at ingen rabat gives på en tilfældig hverdag i juni (langt fra Black Friday).
    [Fact]
    public async Task BlackFridayStrategy_RandomWeekday_ReturnsNoDiscount()
    {
        // Arrange
        IDiscountStrategy strategy = new BlackFridayDiscountStrategy();
        var input = CreateInput(from: new DateTime(2026, 6, 15, 10, 0, 0));

        // Act
        var result = await strategy.Calculate(input);

        // Assert
        Assert.False(result.IsApplicable);
        Assert.Equal(0m, result.DiscountAmount);
    }
}
