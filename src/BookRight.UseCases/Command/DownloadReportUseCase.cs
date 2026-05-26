using BookRight.Facade.Command;
using BookRight.Facade.Interfaces.UseCase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookRight.UseCases.Command
{
    public class DownloadReportUseCase : IDownloadReportUseCase
    {
        Task IDownloadReportUseCase.Execute(DownloadReportRequest request)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); // Henter skrivebordsstien på computeren programmet kører på
            string fileName = $"Omsætningsrapport-{DateTime.Now:yyyy-MM-dd}.txt";
            string fullPath = Path.Combine(desktopPath, fileName); // Fuld sti til filen der skal gemmes

            using (StreamWriter writer = new StreamWriter(fullPath)) // Opretter og skriver til filen
            {
                writer.WriteLine("Rapport fra " + request.PeriodStart.ToString("dd-MM-yyyy") + " til " + request.PeriodEnd.ToString("dd-MM-yyyy")); // Skriver datointervallet fra valgte datoer øverst i .txt filen
                writer.WriteLine($"Generet den: {request.GeneratedDate}");
                writer.WriteLine("===Omsætning===");
                writer.WriteLine();
                writer.WriteLine($"Estimeret omsætning for perioden = {request.EstimatedRevenue}");
                writer.WriteLine();
                writer.WriteLine($"Estimeret omsætning for perioden = {request.TotalRevenue}");
            }

            return Task.CompletedTask;
        }
    }
    
}
