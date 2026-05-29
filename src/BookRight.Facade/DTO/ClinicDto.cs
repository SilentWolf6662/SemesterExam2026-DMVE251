namespace BookRight.Facade.DTO;

public record ClinicDto(
    Guid Id,
    string StreetName,
    int Zipcode,
    int Rooms,
    List<TimeIntervalDto> WorkingHours);
