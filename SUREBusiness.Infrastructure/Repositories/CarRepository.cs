using SUREBusiness.Core.Common;

namespace SUREBusiness.Infrastructure.Repositories;

public sealed class BaseRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly SUREDbContext _db;

    public BaseRepository(SUREDbContext db)
    {
        _db = db;
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _db.Set<T>().FindAsync([id]);
    }

    public async Task AddAsync(T entity)
    {
        await _db.Set<T>().AddAsync(entity);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _db.Set<T>().Update(entity);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _db.Set<T>().Remove(entity);
        await _db.SaveChangesAsync();
    }
}