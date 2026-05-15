using BookRight.Facade.Command;

namespace BookRight.Facade.Interfaces.UseCase;

public interface IBookAppointment
{
    Task Execute(BookAppointmentRequest request);
}
