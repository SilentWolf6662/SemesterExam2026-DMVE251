using BookRight.Domain.Exceptions;
using BookRight.Facade.Command;
using BookRight.Facade.Interfaces.UseCase;
using BookRight.UseCases.Repositories;

namespace BookRight.UseCases.Command;

public class UpdatePreferredPractitionerUseCase : IUpdatePreferredPractitionerUseCase
{
    private readonly IPatientRepository _patientRepo;
    private readonly IPractitionerRepository _practitionerRepo;

    public UpdatePreferredPractitionerUseCase(IPatientRepository patientRepo, IPractitionerRepository practitionerRepo)
    {
        _patientRepo = patientRepo;
        _practitionerRepo = practitionerRepo;
    }

    async Task IUpdatePreferredPractitionerUseCase.Execute(UpdatePreferredPractitionerRequest request)
    {
        var patient = await _patientRepo.GetByIdAsync(request.PatientId)
            ?? throw new NotFoundException($"Patient {request.PatientId} ikke fundet");

        _ = await _practitionerRepo.GetByIdAsync(request.NewPractitionerId)
            ?? throw new NotFoundException($"Behandler {request.NewPractitionerId} ikke fundet");

        patient.UpdatePreferredPractitioner(request.NewPractitionerId);

        await _patientRepo.SaveAsync();
    }
}
