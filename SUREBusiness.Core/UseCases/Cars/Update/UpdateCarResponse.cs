using SUREBusiness.Core.Common;
using SUREBusiness.Core.Entities;

namespace SUREBusiness.Core.UseCases.Cars.Update;

public sealed class UpdateCarResponse : UseCaseResponse<Car>
{
    public UpdateCarResponse(Car result) : base(result)
    {
    }

    public UpdateCarResponse(string errorMessages) : base(errorMessages)
    {
    }
}
