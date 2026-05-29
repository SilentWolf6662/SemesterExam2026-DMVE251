# BookRight

Semesterprojekt 2026 — bookingsystem til klinikker bygget med Blazor Server og .NET 10.

## Hvad skal du bruge

- .NET 10 SDK
- En SQL Server database (Express virker fint)

## Kom i gang

### Connection string

`appsettings.json` er ikke med i repo'et fordi den indeholder passwords. Kopiér template-filen og udfyld dine egne værdier:

```bash
cp src/BookRight.Blazor/appsettings.template.json src/BookRight.Blazor/appsettings.json
```

Åbn filen og sæt din connection string ind under `DefaultConnection`.

Sæt `"SeedData": true` hvis du vil have testdata i databasen med det samme.

### Kør projektet

```bash
dotnet run --project src/BookRight.Blazor
```

Databasen og tabellerne oprettes automatisk første gang.

Hvis du vil køre EF migrations manuelt skal du først sætte en miljøvariabel med din connection string, da `AppDbContextFactory` læser den derfra:

```bash
# Windows (PowerShell)
$env:BOOKRIGHT_CONNECTION_STRING = "Server=...;Database=...;User Id=...;Password=...;TrustServerCertificate=True"

dotnet ef database update --project src/BookRight.Infrastructure --startup-project src/BookRight.Blazor

# Mac/Linux
export BOOKRIGHT_CONNECTION_STRING="Server=...;Database=...;User Id=...;Password=...;TrustServerCertificate=True"

dotnet ef database update --project src/BookRight.Infrastructure --startup-project src/BookRight.Blazor
```

## Projektstruktur

Projektet følger Clean Architecture med et lag per projekt:

- `BookRight.Domain` — entiteter og domæneregler
- `BookRight.Facade` — DTOs og interfaces
- `BookRight.UseCases` — forretningslogik og prisberegning
- `BookRight.Infrastructure` — database, repositories og seed data
- `BookRight.Blazor` — selve UI'en
