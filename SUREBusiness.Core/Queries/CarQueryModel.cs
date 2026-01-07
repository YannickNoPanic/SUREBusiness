using SUREBusiness.Core.Entities;

namespace SUREBusiness.Core.Queries;

public sealed class CarQueryModel
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 25;

    public string? CurrentHolder { get; init; }
    public CarStatus? Status { get; init; }
}