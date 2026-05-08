using BookRight.Domain.Entities;
using BookRight.Domain.ValueObjects;

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

    /// <summary>
    /// Opretter en Appointment uden overlap mod eksisterende aftaler.
    /// Standard-tidsinterval er 09:00-10:00.
    /// </summary>
    private static Appointment CreateAppointment(int start = 9, int end = 10)
    {
        return Appointment.Create(CreateInterval(start, end), TreatmentTypeId, PatientId, PractitionerId, [], []);
    }

    // ── Oprettelse ────────────────────────────────────────────────────────────

    // Tester at alle felter (tidsinterval, behandlingstype, patient og behandler) sættes
    // korrekt, når en appointment oprettes med gyldige data.
    [Fact]
    public void Create_AppointmentWithValidData_AllFieldsAreSetCorrectly()
    {
    }

    // Tester at en nyoprettet appointment altid starter med status "Booked".
    [Fact]
    public void Create_Appointment_DefaultStatusIsBooked()
    {
    }

    // Tester at oprettelse af en appointment kaster DomainException,
    // hvis patienten allerede har en aktiv appointment i det samme tidsrum.
    [Fact]
    public void Create_Appointment_WithPatientOverlap_ThrowsDomainException()
    {
    }

    // Tester at oprettelse af en appointment kaster DomainException,
    // hvis behandleren allerede har en aktiv appointment i det samme tidsrum.
    [Fact]
    public void Create_Appointment_WithPractitionerOverlap_ThrowsDomainException()
    {
    }

    // Tester at en annulleret appointment ikke tæller som aktiv,
    // og derfor ikke blokerer oprettelse af en ny appointment i samme tidsrum.
    [Fact]
    public void Create_Appointment_CancelledOverlapIsIgnored()
    {
    }

    // Tester at to appointments der ligger umiddelbart efter hinanden (slut == næstes start)
    // ikke betragtes som overlappende — kanttilfældet er eksklusivt.
    [Fact]
    public void Create_Appointment_AdjacentTimeSlotsDoNotOverlap()
    {
    }

    // ── UpdateTreatmentType ───────────────────────────────────────────────────

    // Tester at behandlingstypen kan opdateres på en aktiv (Booked) appointment,
    // og at TreatmentTypeId efterfølgende indeholder den nye type.
    [Fact]
    public void UpdateTreatmentType_WhenActive_UpdatesTreatmentTypeId()
    {
    }

    // Tester at opdatering af behandlingstype kaster DomainException
    // når appointmenten allerede er annulleret.
    [Fact]
    public void UpdateTreatmentType_WhenCancelled_ThrowsDomainException()
    {
    }

    // Tester at opdatering af behandlingstype kaster DomainException
    // når appointmenten allerede er markeret som gennemført.
    [Fact]
    public void UpdateTreatmentType_WhenCompleted_ThrowsDomainException()
    {
    }

    // Tester at opdatering af behandlingstype kaster DomainException
    // når appointmenten er markeret som NoShow (patienten mødte ikke op).
    [Fact]
    public void UpdateTreatmentType_WhenNoShow_ThrowsDomainException()
    {
    }

    // ── Statusændringer ───────────────────────────────────────────────────────

    // Tester at Cancel() ændrer appointmentens status til Cancelled.
    [Fact]
    public void Cancel_SetsStatusToCancelled()
    {
    }

    // Tester at Complete() ændrer appointmentens status til Completed.
    [Fact]
    public void Complete_SetsStatusToCompleted()
    {
    }

    // Tester at NoOneShowed() ændrer appointmentens status til NoShow.
    [Fact]
    public void NoOneShowed_SetsStatusToNoShow()
    {
    }

    // ── IsActive ──────────────────────────────────────────────────────────────

    // Tester at IsActive returnerer true for en nyoprettet appointment med status Booked.
    [Fact]
    public void IsActive_ReturnsTrue_WhenStatusIsBooked()
    {
    }

    // Tester at IsActive returnerer false efter at en appointment er annulleret.
    [Fact]
    public void IsActive_ReturnsFalse_WhenCancelled()
    {
    }

    // Tester at IsActive returnerer false efter at en appointment er markeret som gennemført.
    [Fact]
    public void IsActive_ReturnsFalse_WhenCompleted()
    {
    }

    // Tester at IsActive returnerer false efter at en appointment er markeret som NoShow.
    [Fact]
    public void IsActive_ReturnsFalse_WhenNoShow()
    {
    }
}
