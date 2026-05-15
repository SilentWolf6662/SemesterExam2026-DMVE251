using BookRight.Facade.DTO;

namespace BookRight.Facade.Command
{
    public record CreateClinicRequest(
        string StreetName,
        int Zipcode, 
        List<TimeIntervalDto> WorkingHours, 
        int Rooms);
}