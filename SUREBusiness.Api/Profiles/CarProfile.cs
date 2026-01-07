using SUREBusiness.Api.Dtos;
using SUREBusiness.Core.Entities;
using SUREBusiness.Core.UseCases.Cars.Add;
using SUREBusiness.Core.UseCases.Cars.Update;

namespace SUREBusiness.Api.Profiles;

public static class CarProfile
{
    public static CarDto ToDto(this Car car)
        => new(
            Id: car.Id,
            LicensePlate: car.LicensePlate,
            Color: car.Color,
            BuildYear: car.BuildYear,
            BorrowedTo: car.BorrowedTo,
            Status: car.Status.ToDisplayString(), // zie hieronder
            Remarks: car.Comments ?? []);

    public static AddCarRequest ToRequest(this CreateCarDto dto)
        => new(
            LicensePlate: dto.LicensePlate.Trim().ToUpperInvariant(),
            Color: dto.Color,
            BuildYear: dto.BuildYear,
            Remarks: dto.Remarks ?? new());

    public static UpdateCarRequest ToRequest(this UpdateCarDto dto, int id)
        => new(
            Id: id,
            Color: dto.Color,
            BuildYear: dto.BuildYear,
            BorrowedTo: string.IsNullOrWhiteSpace(dto.BorrowedTo) ? null : dto.BorrowedTo.Trim(),
            Status: dto.Status,
            RemarksToAdd: dto.RemarksToAdd ?? []);

    public static string ToDisplayString(this CarStatus status) => status switch
    {
        CarStatus.Available => "Beschikbaar",
        CarStatus.Borrowed => "Uitgeleend",
        CarStatus.InMaintenance => "In reparatie",
        CarStatus.Sold => "Verkocht",
        CarStatus.Ordered => "In bestelling",
        _ => status.ToString()
    };
}
