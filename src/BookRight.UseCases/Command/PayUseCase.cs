using BookRight.Domain.Entities;
using BookRight.Domain.Exceptions;
using BookRight.Facade.Command;
using BookRight.UseCases.Repositories;
using BookRight.UseCases.Services;

namespace BookRight.UseCases.Command;

public class PayUseCase
{
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly IPatientRepository _patientRepo;
    private readonly PricingService _pricingService;

    public PayUseCase(IAppointmentRepository appointmentRepo, IPatientRepository patientRepo, PricingService pricingService)
    {
        _appointmentRepo = appointmentRepo;
        _patientRepo = patientRepo;
        _pricingService = pricingService;
    }

    public async Task Execute(PayRequest request)
    {
        // Hent appointment fra repository
        var appointment = await _appointmentRepo.GetByIdAsync(request.AppointmentId);

        // Hvis appointment ikke findes, kast en NotFoundException
        if (appointment == null) throw new NotFoundException("Appointment kunne ikke findes");

        // Find den endelige pris med rabat og evt. overtidstillæg i PricingService
        decimal finalPrice = await _pricingService.Calculate(appointment);
        // Opdater prisen i appointment
        appointment.ApplyFinalPrice(finalPrice);
        // Opdater appointment status til Completed
        appointment.Complete(appointment.Note);
    }
}
