using SUREBusiness.Core.Common;
using SUREBusiness.Core.Entities;

public sealed class AddCarResponse : UseCaseResponse<Car>
{
    public AddCarResponse(string errorMessages) : base(errorMessages)
    {
    }

    public AddCarResponse(Car result) : base(result)
    {
    }
}
