using BookRight.Facade.Command;

namespace BookRight.Facade.Interfaces.UseCase;

public interface IChangeStatus
{
    Task Execute(ChangeStatusRequest request);
}