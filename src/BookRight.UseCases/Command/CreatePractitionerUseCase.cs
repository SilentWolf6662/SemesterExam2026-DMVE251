using BookRight.Domain.Entities;
using BookRight.Domain.Enums;
using BookRight.Facade.Command;
using BookRight.Facade.Interfaces.UseCase;
using BookRight.UseCases.Repositories;

namespace BookRight.UseCases.Command;

public class CreatePractitionerUseCase : ICreatePractitionerUseCase
{
    private readonly IPractitionerRepository _practitionerRepo;

    public CreatePractitionerUseCase(IPractitionerRepository practitionerRepo)
    {
        _practitionerRepo = practitionerRepo;
    }

    async Task ICreatePractitionerUseCase.Execute(CreatePractitionerRequest request)
    {
        // Parse autorisationstypen fra en streng til den tilsvarende enum-værdi, så domænet arbejder med stærkt typede værdier
        var authorization = Enum.Parse<AuthorizationType>(request.Authorization, ignoreCase: true); // ignoreCase så input ikke er case-sensitiv

        // Opret behandleren via factory-metoden på Practitioner, som sørger for validering af data
        var practitioner = Practitioner.Create(
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Email,
            authorization,
            request.AuthorizationNumber);

        // Tilføj den nye behandler til repository og gem ændringerne i databasen
        await _practitionerRepo.AddAsync(practitioner);
        await _practitionerRepo.SaveAsync();
    }
}
