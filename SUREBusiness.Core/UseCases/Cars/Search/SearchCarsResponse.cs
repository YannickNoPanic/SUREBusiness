using SUREBusiness.Core.Common;
using SUREBusiness.Core.Entities;

namespace SUREBusiness.Core.UseCases.Cars.Search;

public sealed class SearchCarsResponse : UseCaseResponse<PagedResult<Car>>
{
    public SearchCarsResponse(PagedResult<Car> result) : base(result)
    {
    }
    public SearchCarsResponse(string errorMessages) : base(errorMessages)
    {
    }
}
