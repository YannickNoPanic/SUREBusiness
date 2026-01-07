namespace SUREBusiness.Api.Dtos;

public record CreateCarDto(
    string LicensePlate,
    string Color,
    int BuildYear,
    List<string>? Remarks = null);
