using BookRight.Facade.Command;

namespace BookRight.Facade.Interfaces.UseCase;

public interface IUpdatePreferredPractitionerUseCase
{
    Task Execute(UpdatePreferredPractitionerRequest request);
}
