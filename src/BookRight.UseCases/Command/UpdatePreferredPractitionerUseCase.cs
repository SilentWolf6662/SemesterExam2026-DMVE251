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
        // Hent patienten – vi har brug for selve objektet, da vi skal kalde en metode på det
        var patient = await _patientRepo.GetByIdAsync(request.PatientId)
            ?? throw new NotFoundException($"Patient {request.PatientId} ikke fundet"); // Kast NotFoundException hvis patienten ikke findes

        // Tjek at den nye behandler eksisterer, inden vi sætter referencen på patienten
        _ = await _practitionerRepo.GetByIdAsync(request.NewPractitionerId)
            ?? throw new NotFoundException($"Behandler {request.NewPractitionerId} ikke fundet"); // Kast NotFoundException hvis behandleren ikke findes

        // Opdater patientens foretrukne behandler via domænemetoden, som håndterer forretningslogikken
        patient.UpdatePreferredPractitioner(request.NewPractitionerId);

        // Gem ændringerne i databasen – ingen AddAsync da patienten allerede eksisterer
        await _patientRepo.SaveAsync();
    }
}
