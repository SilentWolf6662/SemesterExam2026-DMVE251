namespace BookRight.Facade.DTO;

// ClinicOccupancyDto bruges til at vise belægningsdata på forsiden.
// PeakConcurrentRooms er det højeste antal samtidige aftaler i løbet af dagen
// og bruges som proxy for rumforbrug — én aftale = ét rum.
// OccupancyPercent er beregnet som PeakConcurrentRooms / TotalRooms * 100, max 100.
public record ClinicOccupancyDto(
    Guid ClinicId,
    string Address,
    int TotalRooms,
    int ActiveAppointmentsToday,
    int PeakConcurrentRooms,
    double OccupancyPercent);
