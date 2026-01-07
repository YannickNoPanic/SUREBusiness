namespace SUREBusiness.Api.Dtos;

public record CarDto(
    int Id,
    string LicensePlate,
    string Color,
    int BuildYear,
    string? BorrowedTo,
    string Status,            
    List<string> Remarks);

public enum CarStatusDto
{
    Borrowed,
    Available,
    InMaintenance,
    Sold,
    Ordered
}
