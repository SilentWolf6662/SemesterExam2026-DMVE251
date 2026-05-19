using BookRight.Domain.Entities;
using BookRight.Domain.ValueObjects;
using BookRight.Facade.Command;
using BookRight.Facade.Interfaces.UseCase;
using BookRight.UseCases.Repositories;

namespace BookRight.UseCases.Command;

public class CreateClinicUseCase : ICreateClinicUseCase
{
    private readonly IClinicRepository _clinicRepo;

    public CreateClinicUseCase(IClinicRepository clinicRepo)
    {
        _clinicRepo = clinicRepo;
    }

    async Task ICreateClinicUseCase.Execute(CreateClinicRequest request)
    {
        // Opbyg de value objects der er nødvendige for at oprette en klinik
        var address = new Address(request.StreetName, request.Zipcode); // Pak adressedata ind i et Address value object

        // Tilføj åbningstider og lukketider som TimeInterval value objects
        var workingHours = request.WorkingHours
            .Select(w => new TimeInterval(w.Start, w.End)) // Konverter hver åbningstid til et TimeInterval value object
            .ToList(); // Lig dem i en liste, da Clinic forventer en liste af TimeInterval for åbningstider

        // Opret klinikken via factory-metoden på Clinic, som sørger for validering af data
        var clinic = Clinic.Create(address, workingHours, request.Rooms);

        await _clinicRepo.AddAsync(clinic); // Tilføj den nye klinik til repository
        await _clinicRepo.SaveAsync(); // Gem ændringerne i databasen
    }
}
