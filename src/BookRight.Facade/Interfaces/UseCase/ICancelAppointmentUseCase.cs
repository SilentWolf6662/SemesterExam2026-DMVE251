using BookRight.Facade.Command;

namespace BookRight.Facade.Interfaces.UseCase;

public interface ICancelAppointmentUseCase
{
    Task Execute(CancelAppointmentRequest request);
}