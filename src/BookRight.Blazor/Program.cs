using BookRight.Blazor.Components;
using BookRight.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BookRight.Blazor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddInfrastructure(builder.Configuration);

            var app = builder.Build();

            if (app.Configuration.GetValue<bool>("SeedData"))
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                if (db.Clinics.Any())
                {
                    db.Appointments.ExecuteDelete();
                    db.Patients.ExecuteDelete();
                    db.Practitioners.ExecuteDelete();
                    db.TreatmentTypes.ExecuteDelete();
                    db.Clinics.ExecuteDelete();
                }
                new SeedData().Initialize(db);
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
