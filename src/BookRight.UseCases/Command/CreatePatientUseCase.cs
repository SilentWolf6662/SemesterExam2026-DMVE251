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
        var patient = Patient.Create(
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Email,
            request.Birthday,
            new Address(request.StreetName, request.Zipcode),
            request.Note,
            request.PreferredPractitioner ?? Guid.Empty);

        await _patientRepo.AddAsync(patient);
        await _patientRepo.SaveAsync();
    }
}
