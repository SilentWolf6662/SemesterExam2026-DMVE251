using BookRight.Facade.Interfaces.Queries;
using BookRight.Facade.Interfaces.UseCase;
using BookRight.Infrastructure.Query;
using BookRight.Infrastructure.Repository;
using BookRight.UseCases.Command;
using BookRight.Domain.Discount.DiscountStrategy;
using BookRight.UseCases.Discount;
using BookRight.UseCases.Discount.DiscountStrategy;
using BookRight.UseCases.Repositories;
using BookRight.UseCases.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookRight.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repositories (Scoped)
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IClinicRepository, ClinicRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IPractitionerRepository, PractitionerRepository>();
        services.AddScoped<ITreatmentTypeRepository, TreatmentTypeRepository>();

        // Use Cases (Scoped)
        // Clinic Use Cases
        services.AddScoped<ICreateClinicUseCase, CreateClinicUseCase>();

        // Patient Use Cases
        services.AddScoped<ICreatePatientUseCase, CreatePatientUseCase>();
        services.AddScoped<IUpdatePreferredPractitionerUseCase, UpdatePreferredPractitionerUseCase>();

        // Practitioner Use Cases
        services.AddScoped<ICreatePractitionerUseCase, CreatePractitionerUseCase>();

        // Pricing
        services.AddScoped<IDiscountStrategy, BlackFridayDiscountStrategy>();
        services.AddScoped<IDiscountStrategy, BirthdayDiscountStrategy>();
        services.AddScoped<IDiscountStrategy, BronzeLoyaltyDiscountStrategy>();
        services.AddScoped<IDiscountStrategy, SilverLoyaltyDiscountStrategy>();
        services.AddScoped<IDiscountStrategy, GoldLoyaltyDiscountStrategy>();
        services.AddScoped<PricingService>();

        // Appointment Use Cases
        services.AddScoped<IBookAppointmentUseCase, BookAppointmentUseCase>();
        services.AddScoped<IBookCombinedAppointment, BookCombinedAppointmentUseCase>();
        services.AddScoped<IChangeStatusUseCase, ChangeStatusUseCase>();

        // Queries (Scoped)
        services.AddScoped<IAppointmentQueries, AppointmentQueriesImpl>();
        services.AddScoped<IClinicQueries, ClinicQueriesImpl>();
        services.AddScoped<IPatientQueries, PatientQueriesImpl>();
        services.AddScoped<IPractitionerQueries, PractitionerQueriesImpl>();
        services.AddScoped<ITreatmentTypeQueries, TreatmentTypeQueriesImpl>();
        services.AddScoped<IPricePreviewQueries, PricePreviewQueriesImpl>();

        return services;
    }
}
