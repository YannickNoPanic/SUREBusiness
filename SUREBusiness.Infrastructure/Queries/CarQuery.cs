using Microsoft.EntityFrameworkCore;
using SUREBusiness.Core.Common;
using SUREBusiness.Core.Entities;
using SUREBusiness.Core.Queries;

namespace SUREBusiness.Infrastructure.Queries;

public sealed class CarQuery : ICarQuery
{
    private readonly SUREDbContext _db;

    public CarQuery(SUREDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<Car>> QueryAsync(CarQueryModel model)
    {
        IQueryable<Car> query = _db.Cars.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(model.CurrentHolder))
            query = query.Where(c => c.BorrowedTo == model.CurrentHolder);

        if (model.Status.HasValue)
            query = query.Where(c => c.Status == model.Status.Value);

        var totalCount = await query.CountAsync();

        query = query
            .OrderBy(c => c.LicensePlate)
            .Skip((model.Page - 1) * model.PageSize)
            .Take(model.PageSize);

        var items = await query.ToListAsync();

        return new PagedResult<Car>
        {
            Items = items,
            TotalCount = totalCount,
            Page = model.Page,
            PageSize = model.PageSize
        };
    }
}