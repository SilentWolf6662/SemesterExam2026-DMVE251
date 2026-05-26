using BookRight.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookRight.Infrastructure;

public class AppDbContext : DbContext
{

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Clinic> Clinics => Set<Clinic>();
    public DbSet<Practitioner> Practitioners => Set<Practitioner>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<TreatmentType> TreatmentTypes => Set<TreatmentType>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigurePatient(modelBuilder);
        ConfigureClinic(modelBuilder);
        ConfigureAppointment(modelBuilder);
        ConfigurePractitioner(modelBuilder);
        ConfigureTreatmentType(modelBuilder);
    }

    private static void ConfigurePatient(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(entity =>
        {
            // Address er Value Object så vi bruger ComplexProperty med ToJson til map det til JSON i database
            entity.ComplexProperty(p => p.PatientAddress, a => a.ToJson());

            // Unik email
            entity.HasIndex(p => p.Email).IsUnique();
        });
    }

    private static void ConfigureClinic(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Clinic>(entity =>
        {
            // Address er Value Object så vi bruger ComplexProperty med ToJson til map det til JSON i database
            entity.ComplexProperty(c => c.ClinicAddress, a => a.ToJson());

            // List<TimeInterval> kan ikke mappes implicit, så vi bruger ComplexProperty med ToJson til map det til JSON i database
            // Gemmes som JSON
            entity.ComplexCollection(c => c.WorkingHours, t => t.ToJson());
        });
    }

    private static void ConfigureAppointment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            // TimeInterval er Value Object så vi bruger ComplexProperty med ToJson til map det til JSON i database
            entity.ComplexProperty(c => c.TimeInterval, t => t.ToJson());

            // Enum gemmes som string
            entity.Property(a => a.Status).HasConversion<string>();

            // Laver decimal precision med max 2 decimaler og 18 cifre
            entity.Property(a => a.Price).HasColumnType("decimal(18,2)");
        });
    }

    private static void ConfigurePractitioner(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Practitioner>(entity =>
        {
            // Enum gemmes som string
            entity.Property(p => p.Authorization).HasConversion<string>();

            // List<Guid>
            entity.PrimitiveCollection(p => p.Clinics);

            entity.PrimitiveCollection(p => p.Appointments);

            // Unik email
            entity.HasIndex(p => p.Email).IsUnique();
        });
    }

    private static void ConfigureTreatmentType(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TreatmentType>(entity =>
        {
            // Enum gemmes som læsbar streng i databasen frem for et tal
            entity.Property(t => t.AuthorizationType).HasConversion<string>();

            // Prices er en List<TreatmentPrice> (value objects).
            // ComplexCollection med ToJson gemmer listen som én JSON-kolonne i TreatmentTypes-tabellen —
            // samme mønster som WorkingHours på Clinic. Ingen separat pristeabel er nødvendig.
            entity.ComplexCollection(t => t.Prices, p => p.ToJson());
        });
    }

}