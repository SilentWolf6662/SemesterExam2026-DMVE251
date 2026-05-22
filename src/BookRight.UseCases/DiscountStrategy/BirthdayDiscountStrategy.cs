using BookRight.Domain.Entities;
using BookRight.Domain.Exceptions;
using BookRight.UseCases.Repositories;

namespace BookRight.Domain.Discount;

public class BirthdayDiscountStrategy : IDiscountStrategy
{
    public string Name => "Fødselsdagsrabat";
    public decimal DiscountRate => 0.10m; // 10% rabat

    private readonly IPatientRepository _patientRepository;

    public BirthdayDiscountStrategy(IPatientRepository patientRepository)   
    {
        _patientRepository = patientRepository;
    }

    async Task<CalculatedDiscount> IDiscountStrategy.Calculate(decimal currentPrice, Appointment appointment)
    {
        if (await IsBirthday(appointment))
        {
            return new CalculatedDiscount(
                Name,
                Math.Round(currentPrice * DiscountRate, 2)
            );
        }
        else
        {
            return new CalculatedDiscount(
                Name,
                0
            );
        }
    }

    private async Task<bool> IsBirthday(Appointment appointment)
    {
        // Tjek om måneden for appointmentDate matcher måneden for patientBirthDate
        var patientBirthday = await PatientBirthday(appointment);
        return appointment.TimeInterval.Start.Month == patientBirthday.Month;
    }

    private async Task<DateTime> PatientBirthday(Appointment appointment)
    {
        var patient = await _patientRepository.GetByIdAsync(appointment.PatientId);
        
        if (patient == null) throw new NotFoundException("Patient kunne ikke findes");
        
        return patient.Birthday;
    }
}
