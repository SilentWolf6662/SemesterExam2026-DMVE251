using BookRight.Domain.Entities;
using BookRight.Domain.Enums;
using BookRight.Domain.Exceptions;
using BookRight.Domain.ValueObjects;
using System.Net.NetworkInformation;

namespace BookRight.Domain.Tests.Entities;

/// <summary>
/// Tests for Appointment-entiteten (aggregat-rod).
///
/// Dækker:
///   - Oprettelse via factory-metoden Create() inkl. overlap-validering
///   - Opdatering af behandlingstype (UpdateTreatmentType)
///   - Statusændringer: Cancel, Complete, NoOneShowed
///   - IsActive-egenskaben
/// </summary>
public class AppointmentTests
{
    // ── Hjælpe-data ───────────────────────────────────────────────────────────

    private static readonly Guid TreatmentTypeId = Guid.NewGuid();
    private static readonly Guid PatientId = Guid.NewGuid();
    private static readonly Guid PractitionerId = Guid.NewGuid();

    // Ekstra hjælpe-data til simulering af overlap-tests
    private static readonly Guid ExtraTreatmentTypeId = Guid.NewGuid();
    private static readonly Guid ExtraPatientId = Guid.NewGuid();
    private static readonly Guid ExtraPractitionerId = Guid.NewGuid();

    /// <summary>
    /// Opretter et TimeInterval på en fast testdato mellem de angivne timer.
    /// Bruges til at holde tidsinterval-oprettelse kortfattet i tests.
    /// </summary>
    private static TimeInterval CreateInterval(int startTime, int endTime)
    {
        return new TimeInterval(
            new DateTime(2026, 6, 1, startTime, 0, 0),
            new DateTime(2026, 6, 1, endTime, 0, 0)
        );
    }

    // ── Oprettelse ────────────────────────────────────────────────────────────

    // Tester at alle felter (tidsinterval, behandlingstype, patient og behandler) sættes
    // korrekt, når en appointment oprettes med gyldige data.
    [Fact]
    public void Create_AppointmentWithValidData_AllFieldsAreSetCorrectly()
    {
        // Arrange
        // Arranger nødvendige data til oprettelsen af appointment
        var interval = CreateInterval(9, 10);

        // Act
        // Opret appointment med hjælpe-data
        var appointment = Appointment.Create(interval, TreatmentTypeId, PatientId, PractitionerId, [], []);

        // Assert
        // Tjek om hjælpe-data er blevet sat ind i appointment attributterne, ved at sætte de forventede værdier lig med hinanden
        Assert.Equal(interval, appointment.TimeInterval);
        Assert.Equal(TreatmentTypeId, appointment.TreatmentTypeId);
        Assert.Equal(PatientId, appointment.PatientId);
        Assert.Equal(PractitionerId, appointment.PractitionerId);
    }

    // Tester at en nyoprettet appointment altid starter med status "Booked".
    [Fact]
    public void Create_Appointment_DefaultStatusIsBooked()
    {
        // Arrange
        var interval = CreateInterval(9, 10);

        // Act
        var appointment = Appointment.Create(interval, TreatmentTypeId, PatientId, PractitionerId, [], []);

        // Assert
        Assert.Equal(AppointmentStatus.Booked, appointment.Status);
    }

    // Tester at oprettelsen af en appointment kaster DomainException,
    // hvis patienten allerede har en aktiv appointment i det samme tidsrum.
    [Fact]
    public void Create_Appointment_WithPatientOverlap_ThrowsDomainException()
    {
        // Arrange
        var interval = CreateInterval(9, 10);
        var interval2 = CreateInterval(9, 10); // Samme tidsinterval for overlap

        // Act & Assert
        var appointment = Appointment.Create(interval, TreatmentTypeId, PatientId, PractitionerId, [], []);

        Assert.Throws<DomainException>(() =>
            Appointment.Create(interval2, ExtraTreatmentTypeId, PatientId, ExtraPractitionerId, [appointment], [])
        );
    }

    // Tester at oprettelse af en appointment kaster DomainException,
    // hvis behandleren allerede har en aktiv appointment i det samme tidsrum.
    [Fact]
    public void Create_Appointment_WithPractitionerOverlap_ThrowsDomainException()
    {
        // Arrange
        var interval = CreateInterval(9, 10);
        var interval2 = CreateInterval(9, 10); // Samme tidsinterval for overlap

        // Act & Assert
        var appointment = Appointment.Create(interval, TreatmentTypeId, PatientId, PractitionerId, [], []);

        Assert.Throws<DomainException>(() =>
            Appointment.Create(interval2, ExtraTreatmentTypeId, ExtraPatientId, PractitionerId, [appointment], [])
        );
    }

    // Tester at en annulleret appointment ikke tæller som aktiv,
    // og derfor ikke blokerer oprettelse af en ny appointment i samme tidsrum.
    [Fact]
    public void Create_Appointment_CancelledOverlapIsIgnored()
    {
        // Arrange
        var interval = CreateInterval(9, 10);
        var interval2 = CreateInterval(9, 10); // Samme tidsinterval for overlap

        // Act
        var appointment = Appointment.Create(interval, TreatmentTypeId, PatientId, PractitionerId, [], []);
        appointment.Cancel(); // Annuller appointment for at simulere en anullering

        var newAppointment = Appointment.Create(interval2, ExtraTreatmentTypeId, PatientId, PractitionerId, [appointment], []);

        // Assert
        Assert.Equal(AppointmentStatus.Cancelled, appointment.Status); // Tjek at den første appointment er annulleret
        Assert.NotNull(newAppointment); // Tjek at den anden appointment blev oprettet UDEN at kaste DomainException
    }

    // Tester at to appointments der ligger umiddelbart efter hinanden (slut == næstes start)
    // ikke betragtes som overlappende — kanttilfældet er eksklusivt.
    [Fact]
    public void Create_Appointment_AdjacentTimeSlotsDoNotOverlap()
    {
        // Arrange
        var interval = CreateInterval(9, 10);
        var interval2 = CreateInterval(10, 11); // Næste tidsinterval starter lige efter det første

        // Act
        var appointment = Appointment.Create(interval, TreatmentTypeId, PatientId, PractitionerId, [], []);
        var newAppointment = Appointment.Create(interval2, ExtraTreatmentTypeId, PatientId, PractitionerId, [appointment], []);

        // Assert
        Assert.NotNull(newAppointment); // Tjek at den anden appointment blev oprettet UDEN at kaste DomainException
    }

    // ── UpdateTreatmentType ───────────────────────────────────────────────────

    // Tester at behandlingstypen kan opdateres på en aktiv (Booked) appointment,
    // og at TreatmentTypeId efterfølgende indeholder den nye type.
    [Fact]
    public void UpdateTreatmentType_WhenActive_UpdatesTreatmentTypeId()
    {
        // Arrange
        var interval = CreateInterval(9, 10);
        var appointment = Appointment.Create(interval, TreatmentTypeId, PatientId, PractitionerId, [], []);

        // Act
        appointment.UpdateTreatmentType(ExtraTreatmentTypeId);

        // Assert
        Assert.Equal(ExtraTreatmentTypeId, appointment.TreatmentTypeId);
    }

    /// <summary>
    /// Returnerer statusændringer der gør appointmenten ugyldig
    /// for opdatering af behandlingstype.
    /// </summary>
    public static IEnumerable<object[]> InvalidStatusActions()
    {
        yield return
        [
            new Action<Appointment>(appointment => appointment.Cancel())
        ];

        yield return
        [
            new Action<Appointment>(appointment => appointment.Complete("Hello World"))
        ];

        yield return
        [
            new Action<Appointment>(appointment => appointment.NoOneShowed())
        ];
    }

    /// <summary>
    /// Verificerer at UpdateTreatmentType kaster DomainException
    /// når appointmenten har en status der ikke tillader ændringer.
    /// </summary>
    /// <param name="changeStatus">
    /// Handling der ændrer appointmentens status.
    /// </param>
    [Theory]
    [MemberData(nameof(InvalidStatusActions))]
    public void UpdateTreatmentType_InvalidStatus_ThrowsDomainException(Action<Appointment> changeStatus)
    {
        // Arrange
        var interval = CreateInterval(9, 10);

        var appointment = Appointment.Create(interval, TreatmentTypeId, PatientId, PractitionerId, [], []);

        // Act
        changeStatus(appointment);

        // Assert
        Assert.Throws<DomainException>(() => appointment.UpdateTreatmentType(ExtraTreatmentTypeId));
    }


    // ── Statusændringer ───────────────────────────────────────────────────────

    // Tester at Cancel() ændrer appointmentens status til Cancelled.
    [Fact]
    public void StatusChanges_WorksAsExpected()
    {
        // Arrange
        var interval = CreateInterval(9, 10);
        var appointment = Appointment.Create(interval, TreatmentTypeId, PatientId, PractitionerId, [], []);

        // Act & Assert
        appointment.Cancel();
        Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
        appointment.Complete("Hello World");
        Assert.Equal(AppointmentStatus.Completed, appointment.Status);
        appointment.NoOneShowed();
        Assert.Equal(AppointmentStatus.NoShow, appointment.Status);
    }
}
