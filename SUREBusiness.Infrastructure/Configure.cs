using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SUREBusiness.Core.Common;
using SUREBusiness.Core.Entities;
using SUREBusiness.Core.Interfaces;
using SUREBusiness.Core.Queries;
using SUREBusiness.Infrastructure.Queries;
using SUREBusiness.Infrastructure.Repositories;
using SUREBusiness.Infrastructure.Services;

namespace SUREBusiness.Infrastructure;

public static class Configure
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<SUREDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SUREDB"))); // default fallback
        services.AddRepositories();
        services.AddQueries();
        services.AddServices(configuration);

        return services;
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IRepository<Car>, BaseRepository<Car>>();
    }

    private static void AddQueries(this IServiceCollection services)
    {
        services.AddScoped<ICarQuery, CarQuery>();
    }

    private static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<ILicensePlateValidator, RdwLicensePlateValidator>(client =>
        {
            client.BaseAddress = new Uri("https://opendata.rdw.nl/");
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        services.AddScoped<ILicensePlateValidator, RdwLicensePlateValidator>();
    }
}