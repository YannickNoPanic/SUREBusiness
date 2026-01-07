using SUREBusiness.Core.Common;

namespace SUREBusiness.Core.Entities;

public class Car : BaseEntity
{
    public required string LicensePlate { get; set; }
    public required string Color { get; set; }
    public required int BuildYear { get; set; }
    public string? BorrowedTo { get; set; }
    public DateTimeOffset? BorrowedDate { get; set; }
    public CarStatus Status { get; set; } = CarStatus.Available;
    public List<string> Comments { get; set; } = new();
}