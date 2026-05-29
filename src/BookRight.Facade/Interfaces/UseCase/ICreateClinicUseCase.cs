using BookRight.Facade.Command;

namespace BookRight.Facade.Interfaces.UseCase;

public interface ICreateClinicUseCase
{
    Task Execute(CreateClinicRequest request);
}