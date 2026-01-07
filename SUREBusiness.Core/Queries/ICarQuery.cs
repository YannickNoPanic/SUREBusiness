using SUREBusiness.Core.Common;
using SUREBusiness.Core.Entities;

namespace SUREBusiness.Core.Queries;


public interface ICarQuery
{
    Task<PagedResult<Car>> QueryAsync(CarQueryModel model);
}
