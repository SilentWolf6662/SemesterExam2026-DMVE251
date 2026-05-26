namespace BookRight.Facade.DTO;

public record ClinicOccupancyDto(
    Guid ClinicId,
    string Address,
    int TotalRooms,
    int ActiveAppointmentsToday,
    int PeakConcurrentRooms,
    double OccupancyPercent);
