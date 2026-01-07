using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SUREBusiness.Core.UseCases.Cars.Add;
using SUREBusiness.Core.UseCases.Cars.Get;
using SUREBusiness.Core.UseCases.Cars.Search;
using SUREBusiness.Core.UseCases.Cars.Update;


namespace SUREBusiness.Core;

public static class Configure
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAddCarUseCase, AddCarUseCase>();
        services.AddScoped<IUpdateCarUseCase, UpdateCarUseCase>();
        services.AddScoped<IGetCarUseCase, GetCarUseCase>();
        services.AddScoped<ISearchCarsUseCase, SearchCarsUseCase>();

        return services;
    }
}