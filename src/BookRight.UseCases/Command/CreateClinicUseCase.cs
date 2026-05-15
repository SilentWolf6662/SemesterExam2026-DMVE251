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
        var address = new Address(request.StreetName, request.Zipcode);
        var workingHours = request.WorkingHours
            .Select(w => new TimeInterval(w.Start, w.End))
            .ToList();

        var clinic = Clinic.Create(address, workingHours, request.Rooms);

        await _clinicRepo.AddAsync(clinic);
        await _clinicRepo.SaveAsync();
    }
}
