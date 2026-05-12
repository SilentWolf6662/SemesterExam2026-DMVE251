using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BookRight.Infrastructure;

// Denne klasse implementerer IDesignTimeDbContextFactory.
// Formålet er at fortælle Entity Framework hvordan AppDbContext
// skal oprettes under design-time, fx når man kører:
// - Add-Migration
// - Update-Database
// - dotnet ef migrations add
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    // Denne metode bliver automatisk kaldt af Entity Framework Tools.
    // Den opretter og returnerer en konfigureret AppDbContext-instans.
    public AppDbContext CreateDbContext(string[] args)
    {
        // Opretter en DbContextOptionsBuilder til AppDbContext.
        // Denne builder bruges til at konfigurere databasen og forbindelsen.
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Konfigurerer Entity Framework til at bruge SQL Server.
        // Denne konfiguration bruges kun under design-time, altså når migrations eller databaseopdateringer køres.
        optionsBuilder.UseSqlServer(
            "Server=.;Database=BookRightDb;Trusted_Connection=True;TrustServerCertificate=True"
        );

        // Returnerer en ny instans af AppDbContext
        // med de konfigurerede options.
        return new AppDbContext(optionsBuilder.Options);
    }
}