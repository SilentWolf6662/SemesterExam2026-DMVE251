using BookRight.Facade.Command;

namespace BookRight.Facade.Interfaces.UseCase;

public interface ICreatePatientUseCase
{
    Task Execute(CreatePatientRequest request);
}