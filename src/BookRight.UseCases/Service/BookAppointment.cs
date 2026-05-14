using BookRight.UseCases.Repositories;

namespace BookRight.UseCases.Service;

public class BookAppointment
{
    private readonly IAppointmentRepository _repository;

    private BookAppointment(IAppointmentRepository repository)
    {
        _repository = repository;
    }
}