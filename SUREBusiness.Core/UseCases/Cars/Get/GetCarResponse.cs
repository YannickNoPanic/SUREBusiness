using SUREBusiness.Core.Common;
using SUREBusiness.Core.Entities;

namespace SUREBusiness.Core.UseCases.Cars.Get;

public sealed class GetCarResponse : UseCaseResponse<Car>
{
    public GetCarResponse(string errorMessages) : base(errorMessages)
    {
    }

    public GetCarResponse(Car result) : base(result)
    {
    }
}
