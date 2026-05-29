using BookRight.Facade.Command;

namespace BookRight.Facade.Interfaces.UseCase;

public interface IChangeStatusUseCase
{
    Task Execute(ChangeStatusRequest request);
}