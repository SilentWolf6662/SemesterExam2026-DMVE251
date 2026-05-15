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
        var authorization = Enum.Parse<AuthorizationType>(request.Authorization, ignoreCase: true);

        var practitioner = Practitioner.Create(
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Email,
            authorization,
            request.AuthorizationNumber);

        await _practitionerRepo.AddAsync(practitioner);
        await _practitionerRepo.SaveAsync();
    }
}
