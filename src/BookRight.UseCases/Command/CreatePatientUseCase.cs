using BookRight.Domain.Entities;
using BookRight.Domain.ValueObjects;
using BookRight.Facade.Command;
using BookRight.Facade.Interfaces.UseCase;
using BookRight.UseCases.Repositories;

namespace BookRight.UseCases.Command;

public class CreatePatientUseCase : ICreatePatientUseCase
{
    private readonly IPatientRepository _patientRepo;

    public CreatePatientUseCase(IPatientRepository patientRepo)
    {
        _patientRepo = patientRepo;
    }

    async Task ICreatePatientUseCase.Execute(CreatePatientRequest request)
    {
        // Opret patienten via factory-metoden på Patient, som sørger for validering af data
        var patient = Patient.Create(
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Email,
            request.Birthday,
            new Address(request.StreetName, request.Zipcode), // Pak adressedata ind i et Address value object
            request.Note,
            request.PreferredPractitioner ?? Guid.Empty); // Hvis ingen foretrukken behandler er angivet, bruges Guid.Empty som standardværdi

        // Tilføj den nye patient til repository og gem ændringerne i databasen
        await _patientRepo.AddAsync(patient);
        await _patientRepo.SaveAsync();
    }
}
