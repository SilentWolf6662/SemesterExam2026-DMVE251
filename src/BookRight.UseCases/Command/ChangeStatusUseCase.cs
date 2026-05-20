using BookRight.Domain.Exceptions;
using BookRight.Facade.Command;
using BookRight.Facade.Interfaces.UseCase;
using BookRight.UseCases.Repositories;

namespace BookRight.UseCases.Command;

public class ChangeStatusUseCase : IChangeStatus
{
    private readonly IAppointmentRepository _appointmentRepo;

    public ChangeStatusUseCase(IAppointmentRepository appointmentRepo)
    {
        _appointmentRepo = appointmentRepo;
    }

    public async Task Execute(ChangeStatusRequest request)
    {
        var booking = await _appointmentRepo.GetByIdAsync(request.AppointmentId);

        // Hvis aftalen ikke findes, kaster vi en NotFoundException
        if (booking == null) throw new NotFoundException("Appointment not found"); 

        switch (request.Status.ToLower())
        {
            case "cancelled":
                booking.Cancel(); // Sæt status til "cancelled" og fjern eventuelle noter
                break;
            case "completed":
                booking.Complete(request.Note); // Sæt status til "completed" og opdater noten
                break;
            case "noshow":
                booking.NoOneShowed(); // Sæt status til "nooneshowed" og fjern eventuelle noter
                break;
            default:
                throw new ArgumentException("Invalid status");
        }

        await _appointmentRepo.SaveAsync();
    }
}
