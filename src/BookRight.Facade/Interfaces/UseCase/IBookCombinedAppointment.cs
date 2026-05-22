using BookRight.Facade.Command;

namespace BookRight.Facade.Interfaces.UseCase;

public interface IBookCombinedAppointment
{
    Task Execute(BookCombinedAppointmentRequest request);
}
