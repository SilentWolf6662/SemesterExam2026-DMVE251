using BookRight.Facade.Interfaces.Queries;
using BookRight.Facade.Interfaces.UseCase;
using BookRight.Infrastructure.Query;
using BookRight.Infrastructure.Repository;
using BookRight.UseCases.Command;
using BookRight.UseCases.Repositories;
using BookRight.UseCases.Service;
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

        // Appointment Use Cases
        services.AddScoped<IBookAppointment, BookAppointment>();

        // Queries (Scoped)
        services.AddScoped<IAppointmentQueries, AppointmentQueriesImpl>();
        services.AddScoped<IClinicQueries, ClinicQueriesImpl>();
        services.AddScoped<IPatientQueries, PatientQueriesImpl>();
        services.AddScoped<IPractitionerQueries, PractitionerQueriesImpl>();
        services.AddScoped<ITreatmentTypeQueries, TreatmentTypeQueriesImpl>();

        return services;
    }
}
