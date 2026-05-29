using BookRight.Facade.Command;

namespace BookRight.Facade.Interfaces.UseCase;

public interface ICreatePractitionerUseCase
{
    Task Execute(CreatePractitionerRequest request);
}
