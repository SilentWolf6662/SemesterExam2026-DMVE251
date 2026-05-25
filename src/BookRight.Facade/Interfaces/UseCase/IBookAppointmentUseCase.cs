using BookRight.Facade.Command;

namespace BookRight.Facade.Interfaces.UseCase;

public interface IBookAppointmentUseCase
{
    Task Execute(BookAppointmentRequest request);
}
