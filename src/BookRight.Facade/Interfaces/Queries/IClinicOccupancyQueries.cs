using BookRight.Facade.DTO;

namespace BookRight.Facade.Interfaces.Queries;

// Definerer læse-operationer til beregning af klinik belægning.
// Adskilt fra IClinicQueries fordi belægning er et tværgående overblikskoncept
// der kombinerer data fra både klinikker og aftaler.
public interface IClinicOccupancyQueries
{
    // Returnerer belægningsdata for alle klinikker baseret på dagens aftaler.
    Task<IReadOnlyList<ClinicOccupancyDto>> GetTodayAsync();
}
