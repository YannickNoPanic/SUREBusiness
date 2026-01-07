using SUREBusiness.Core.Entities;

namespace SUREBusiness.Api.Dtos;

public record UpdateCarDto(
    string? LicensePlate = null,     
    string? Color = null,
    int? BuildYear = null,
    string? BorrowedTo = null,       
    CarStatus? Status = null,        
    List<string>? RemarksToAdd = null);
