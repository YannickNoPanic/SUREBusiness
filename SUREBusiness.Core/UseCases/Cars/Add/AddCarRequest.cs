namespace SUREBusiness.Core.UseCases.Cars.Add;

public record AddCarRequest(
    string LicensePlate,
    string Color,
    int BuildYear,
    List<string>? Remarks = null);