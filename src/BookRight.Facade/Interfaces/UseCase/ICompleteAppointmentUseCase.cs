using BookRight.Facade.Command;

namespace BookRight.Facade.Interfaces.UseCase;

public interface ICompleteAppointmentUseCase
{
    Task Execute(CompleteAppointmentRequest request);
}