using SUREBusiness.Core.Entities;

namespace SUREBusiness.Core.UseCases.Cars.Update;

public record UpdateCarRequest(
    int Id,
    string? Color = null,
    int? BuildYear = null,
    string? BorrowedTo = null,
    CarStatus? Status = null,
    List<string>? RemarksToAdd = null);