using BookRight.Facade.Command;

namespace BookRight.Facade.Interfaces.UseCase;

public interface INoOneShowedAppointmentUseCase
{
    Task Execute(NoOneShowedAppointmentRequest request);
}