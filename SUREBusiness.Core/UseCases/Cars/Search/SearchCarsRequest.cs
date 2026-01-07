using SUREBusiness.Core.Entities;

namespace SUREBusiness.Core.UseCases.Cars.Search;

public record SearchCarsRequest(
    string? CurrentHolder = null, 
    CarStatus? Status = null,
    int Page = 1,
    int PageSize = 25);
