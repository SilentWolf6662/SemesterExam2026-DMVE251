using BookRight.Domain.Entities;
using BookRight.Domain.Enums;
using BookRight.Domain.ValueObjects;
using BookRight.Domain.Discount;
using BookRight.UseCases.Interfaces;
using BookRight.UseCases.Repositories;
using BookRight.UseCases.Services;
using Moq;

namespace BookRight.UseCases.Tests.Services;

// Tests for PricingService.
//
// Dækker:
//   - Calculate: aftens/weekend-tillæg, rabat og komplet PriceBreakdown
//   - CalculateCombined: proportional rabatfordeling på to sammenhængende bookinger

public class PricingServiceTests
{
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock = new();
    private readonly Mock<IPatientRepository> _patientRepoMock = new();
    private readonly Mock<ITreatmentTypeRepository> _treatmentTypeRepoMock = new();
    private readonly Mock<ICampaignRepository> _campaignRepoMock = new();

    // ── Hjælpemetoder ─────────────────────────────────────────────────────────

    // Bygger en PricingService med de angivne strategier og de fælles repo-mocks.
    private PricingService BuildService(params IDiscountStrategy[] strategies) =>
        new(strategies, _appointmentRepoMock.Object, _patientRepoMock.Object, _treatmentTypeRepoMock.Object, _campaignRepoMock.Object);

    // Opsætter patient-, appointment- og kampagne-mocks med neutrale returværdier
    // (ingen omsætning, ingen brugt fødselsdagsrabat, ingen aktiv kampagne) — bruges i de fleste tests.
    private void SetupNeutralMocks(Guid patientId)
    {
        var patient = Patient.Create("Test", "Patient", "12345678", "test@test.dk",
            new DateTime(1985, 3, 14), new Address("Testvej 1", 7100), null, Guid.Empty);

        _patientRepoMock.Setup(r => r.GetByIdAsync(patientId)).ReturnsAsync(patient);
        _appointmentRepoMock.Setup(r => r.GetSumOf12MonthsByPatientIdAsync(patientId, It.IsAny<DateTime>()))
            .ReturnsAsync(0m);
        _appointmentRepoMock.Setup(r => r.GetBirthdayDiscountUsedCountByPatientIdAsync(patientId, It.IsAny<DateTime>()))
            .ReturnsAsync(0);
        _campaignRepoMock.Setup(r => r.GetCampaignForAppointmentTimeAsync(It.IsAny<DateTime>()))
           .ReturnsAsync((Campaign?)null);
    }

    // ── Calculate: overtidstillæg ─────────────────────────────────────────────

    // Tester at en dagtimebooking (hverdagen kl. 10) returnerer basisprisen uden tillæg.
    [Fact]
    public async Task Calculate_DaytimeWeekdayBooking_ReturnsBasePrice()
    {
        // Arrange
        var treatmentTypeId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var treatmentType = TreatmentType.Create("Fysioterapi", AuthorizationType.Physiotherapist, null,
            [new TreatmentPrice(60, 500m)]);
        _treatmentTypeRepoMock.Setup(r => r.GetByIdAsync(treatmentTypeId)).ReturnsAsync(treatmentType);
        SetupNeutralMocks(patientId);
        var service = BuildService();

        // Act
        var result = await service.Calculate(treatmentTypeId, 60, patientId, new DateTime(2026, 6, 1, 10, 0, 0));

        // Assert
        Assert.Equal(500m, result.FinalPrice); // Ingen tillæg, ingen rabat → præcis basispris
    }

    // Tester at en aftenbooking (fra kl. 17) pålægges 15 % tillæg.
    [Fact]
    public async Task Calculate_EveningBooking_AppliesOvertimeSurcharge()
    {
        // Arrange
        var treatmentTypeId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var treatmentType = TreatmentType.Create("Fysioterapi", AuthorizationType.Physiotherapist, null,
            [new TreatmentPrice(60, 500m)]);
        _treatmentTypeRepoMock.Setup(r => r.GetByIdAsync(treatmentTypeId)).ReturnsAsync(treatmentType);
        SetupNeutralMocks(patientId);
        var service = BuildService();

        // Act
        var result = await service.Calculate(treatmentTypeId, 60, patientId, new DateTime(2026, 6, 1, 17, 0, 0));

        // Assert
        Assert.Equal(575m, result.FinalPrice); // 500 * 1.15 = 575 kr
    }

    // Tester at en weekendbooking pålægges 15 % tillæg uanset tidspunktet på dagen.
    [Fact]
    public async Task Calculate_WeekendBooking_AppliesOvertimeSurcharge()
    {
        // Arrange
        var treatmentTypeId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var treatmentType = TreatmentType.Create("Fysioterapi", AuthorizationType.Physiotherapist, null,
            [new TreatmentPrice(60, 500m)]);
        _treatmentTypeRepoMock.Setup(r => r.GetByIdAsync(treatmentTypeId)).ReturnsAsync(treatmentType);
        SetupNeutralMocks(patientId);
        var service = BuildService();

        // Act
        var result = await service.Calculate(treatmentTypeId, 60, patientId, new DateTime(2026, 6, 6, 10, 0, 0)); // Lørdag kl. 10

        // Assert
        Assert.Equal(575m, result.FinalPrice); // 500 * 1.15 = 575 kr
    }

    // ── Calculate: rabat ──────────────────────────────────────────────────────

    // Tester at en enkelt strategi der giver 50 kr rabat resulterer i korrekt slutpris.
    [Fact]
    public async Task Calculate_WithOneStrategy_AppliesDiscount()
    {
        // Arrange
        var treatmentTypeId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var treatmentType = TreatmentType.Create("Fysioterapi", AuthorizationType.Physiotherapist, null,
            [new TreatmentPrice(60, 500m)]);
        _treatmentTypeRepoMock.Setup(r => r.GetByIdAsync(treatmentTypeId)).ReturnsAsync(treatmentType);
        SetupNeutralMocks(patientId);

        var strategyMock = new Mock<IDiscountStrategy>();
        strategyMock.Setup(s => s.Calculate(It.IsAny<DiscountInput>()))
            .ReturnsAsync(new DiscountResult("Testrabat", 50m, true, DiscountType.BronzeLoyalty));

        var service = BuildService(strategyMock.Object);

        // Act
        var result = await service.Calculate(treatmentTypeId, 60, patientId, new DateTime(2026, 6, 1, 10, 0, 0));

        // Assert
        Assert.Equal(450m, result.FinalPrice); // 500 - 50 = 450 kr
    }

    // Tester at den højeste rabat vælges når to strategier returnerer forskellige beløb.
    [Fact]
    public async Task Calculate_WithMultipleStrategies_SelectsBestDiscount()
    {
        // Arrange
        var treatmentTypeId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var treatmentType = TreatmentType.Create("Fysioterapi", AuthorizationType.Physiotherapist, null,
            [new TreatmentPrice(60, 500m)]);
        _treatmentTypeRepoMock.Setup(r => r.GetByIdAsync(treatmentTypeId)).ReturnsAsync(treatmentType);
        SetupNeutralMocks(patientId);

        var lowStrategy = new Mock<IDiscountStrategy>();
        lowStrategy.Setup(s => s.Calculate(It.IsAny<DiscountInput>()))
            .ReturnsAsync(new DiscountResult("Lille rabat", 25m, true, DiscountType.BronzeLoyalty));

        var highStrategy = new Mock<IDiscountStrategy>();
        highStrategy.Setup(s => s.Calculate(It.IsAny<DiscountInput>()))
            .ReturnsAsync(new DiscountResult("Stor rabat", 100m, true, DiscountType.GoldLoyalty));

        var service = BuildService(lowStrategy.Object, highStrategy.Object);

        // Act
        var result = await service.Calculate(treatmentTypeId, 60, patientId, new DateTime(2026, 6, 1, 10, 0, 0));

        // Assert
        Assert.Equal(400m, result.FinalPrice); // 500 - 100 (bedste rabat) = 400 kr
    }

    // Tester at en strategi der returnerer IsApplicable = false ikke tages med i valget.
    [Fact]
    public async Task Calculate_WithInapplicableStrategy_IgnoresDiscount()
    {
        // Arrange
        var treatmentTypeId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var treatmentType = TreatmentType.Create("Fysioterapi", AuthorizationType.Physiotherapist, null,
            [new TreatmentPrice(60, 500m)]);
        _treatmentTypeRepoMock.Setup(r => r.GetByIdAsync(treatmentTypeId)).ReturnsAsync(treatmentType);
        SetupNeutralMocks(patientId);

        var strategyMock = new Mock<IDiscountStrategy>();
        strategyMock.Setup(s => s.Calculate(It.IsAny<DiscountInput>()))
            .ReturnsAsync(new DiscountResult("Ikke gyldig", 100m, false, DiscountType.None)); // IsApplicable = false

        var service = BuildService(strategyMock.Object);

        // Act
        var result = await service.Calculate(treatmentTypeId, 60, patientId, new DateTime(2026, 6, 1, 10, 0, 0));

        // Assert
        Assert.Equal(500m, result.FinalPrice); // Ingen gyldig rabat → basisprisen returneres
    }

    // ── CalculateCombined ─────────────────────────────────────────────────────

    // Tester at to bookinger uden rabat returnerer de korrekte overtidspriser for begge.
    [Fact]
    public async Task CalculateCombined_NoDiscount_ReturnsBothOvertimePrices()
    {
        // Arrange
        var treatment1Id = Guid.NewGuid();
        var treatment2Id = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var from = new DateTime(2026, 6, 1, 10, 0, 0); // Mandag kl. 10 → ingen tillæg

        var type1 = TreatmentType.Create("Fysioterapi", AuthorizationType.Physiotherapist, null,
            [new TreatmentPrice(60, 500m)]);
        var type2 = TreatmentType.Create("Massage", AuthorizationType.Masseur, null,
            [new TreatmentPrice(60, 400m)]);
        _treatmentTypeRepoMock.Setup(r => r.GetByIdAsync(treatment1Id)).ReturnsAsync(type1);
        _treatmentTypeRepoMock.Setup(r => r.GetByIdAsync(treatment2Id)).ReturnsAsync(type2);
        SetupNeutralMocks(patientId);
        var service = BuildService();

        // Act
        var (first, second) = await service.CalculateCombined(treatment1Id, 60, treatment2Id, 60, patientId, from);

        // Assert
        Assert.Equal(500m, first.FinalPrice);
        Assert.Equal(400m, second.FinalPrice);
        Assert.Null(first.BestDiscount);
    }

    // Tester at rabatten fordeles proportionalt mellem de to behandlinger baseret på deres pris.
    // Fx: 1. behandling = 600 kr, 2. behandling = 400 kr → i alt 1.000 kr.
    // En rabat på 100 kr fordeles: 60 kr til 1. (60%) og 40 kr til 2. (40%).
    [Fact]
    public async Task CalculateCombined_WithDiscount_DistributesProportionally()
    {
        // Arrange
        var treatment1Id = Guid.NewGuid();
        var treatment2Id = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var from = new DateTime(2026, 6, 1, 10, 0, 0); // Ingen overtid

        var type1 = TreatmentType.Create("Fysioterapi", AuthorizationType.Physiotherapist, null,
            [new TreatmentPrice(60, 600m)]);
        var type2 = TreatmentType.Create("Massage", AuthorizationType.Masseur, null,
            [new TreatmentPrice(60, 400m)]);
        _treatmentTypeRepoMock.Setup(r => r.GetByIdAsync(treatment1Id)).ReturnsAsync(type1);
        _treatmentTypeRepoMock.Setup(r => r.GetByIdAsync(treatment2Id)).ReturnsAsync(type2);
        SetupNeutralMocks(patientId);

        var strategyMock = new Mock<IDiscountStrategy>();
        strategyMock.Setup(s => s.Calculate(It.IsAny<DiscountInput>()))
            .ReturnsAsync(new DiscountResult("Testrabat", 100m, true, DiscountType.BronzeLoyalty));

        var service = BuildService(strategyMock.Object);

        // Act
        var (first, second) = await service.CalculateCombined(treatment1Id, 60, treatment2Id, 60, patientId, from);

        // Assert
        // 60 % af 100 = 60 → 1. behandling: 600 - 60 = 540 kr
        // 40 % af 100 = 40 → 2. behandling: 400 - 40 = 360 kr
        Assert.Equal(540m, first.FinalPrice);
        Assert.Equal(360m, second.FinalPrice);
        Assert.Equal(DiscountType.BronzeLoyalty, first.BestDiscount?.DiscountType);
    }

    // ── Calculate: komplet PriceBreakdown ─────────────────────────────────────

    // Tester at Calculate returnerer et korrekt PriceBreakdown for en dagtimebooking.
    [Fact]
    public async Task Calculate_DaytimeBooking_ReturnsCorrectBreakdown()
    {
        // Arrange
        var treatmentTypeId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var from = new DateTime(2026, 6, 1, 10, 0, 0); // Ingen aftens/weekend-tillæg
        const int duration = 60;

        var treatmentType = TreatmentType.Create("Fysioterapi", AuthorizationType.Physiotherapist, null,
            [new TreatmentPrice(duration, 500m)]);
        _treatmentTypeRepoMock.Setup(r => r.GetByIdAsync(treatmentTypeId)).ReturnsAsync(treatmentType);
        SetupNeutralMocks(patientId);

        var service = BuildService(); // Ingen rabatstrategier

        // Act
        var result = await service.Calculate(treatmentTypeId, duration, patientId, from);

        // Assert
        Assert.Equal(500m, result.BasePrice);
        Assert.Equal(0m, result.OvertimeSurcharge);      // Ingen tillæg på hverdagsbooking
        Assert.Equal(500m, result.PriceBeforeDiscount);
        Assert.Null(result.BestDiscount);                // Ingen rabat
        Assert.Equal(500m, result.FinalPrice);
    }

    // Tester at Calculate inkluderer aftenstillæg for en booking kl. 17.
    [Fact]
    public async Task Calculate_EveningBooking_IncludesOvertimeSurcharge()
    {
        // Arrange
        var treatmentTypeId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var from = new DateTime(2026, 6, 1, 17, 0, 0); // Kl. 17 → 15 % tillæg
        const int duration = 60;

        var treatmentType = TreatmentType.Create("Fysioterapi", AuthorizationType.Physiotherapist, null,
            [new TreatmentPrice(duration, 500m)]);
        _treatmentTypeRepoMock.Setup(r => r.GetByIdAsync(treatmentTypeId)).ReturnsAsync(treatmentType);
        SetupNeutralMocks(patientId);

        var service = BuildService();

        // Act
        var result = await service.Calculate(treatmentTypeId, duration, patientId, from);

        // Assert
        Assert.Equal(500m, result.BasePrice);
        Assert.Equal(75m, result.OvertimeSurcharge);     // 500 * 0.15 = 75 kr tillæg
        Assert.Equal(575m, result.PriceBeforeDiscount);
        Assert.Equal(575m, result.FinalPrice);
    }

    // Tester at Calculate inkluderer rabat i BestDiscount-feltet.
    [Fact]
    public async Task Calculate_WithDiscount_SetsBestDiscount()
    {
        // Arrange
        var treatmentTypeId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var from = new DateTime(2026, 6, 1, 10, 0, 0);
        const int duration = 60;

        var treatmentType = TreatmentType.Create("Fysioterapi", AuthorizationType.Physiotherapist, null,
            [new TreatmentPrice(duration, 500m)]);
        _treatmentTypeRepoMock.Setup(r => r.GetByIdAsync(treatmentTypeId)).ReturnsAsync(treatmentType);
        SetupNeutralMocks(patientId);

        var strategyMock = new Mock<IDiscountStrategy>();
        strategyMock.Setup(s => s.Calculate(It.IsAny<DiscountInput>()))
            .ReturnsAsync(new DiscountResult("Testrabat", 50m, true, DiscountType.BronzeLoyalty));

        var service = BuildService(strategyMock.Object);

        // Act
        var result = await service.Calculate(treatmentTypeId, duration, patientId, from);

        // Assert
        Assert.NotNull(result.BestDiscount);
        Assert.Equal(50m, result.BestDiscount.DiscountAmount);
        Assert.Equal(450m, result.FinalPrice); // 500 - 50 = 450 kr
    }
}
