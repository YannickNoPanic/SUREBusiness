using Microsoft.EntityFrameworkCore;
using SUREBusiness.Core.Entities;

namespace SUREBusiness.Infrastructure;

public sealed class SUREDbContext(DbContextOptions<SUREDbContext> options) : DbContext(options)
{
    public DbSet<Car> Cars => Set<Car>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(SUREDbContext).Assembly);
    }

}