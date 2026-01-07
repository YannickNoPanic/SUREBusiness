using SUREBusiness.Core.Common;
using SUREBusiness.Core.Entities;

namespace SUREBusiness.Core.UseCases.Cars.Get;

public sealed class GetCarUseCase(IRepository<Car> carRepository) : IGetCarUseCase
{
    private readonly IRepository<Car> _carRepository = carRepository;

    public async Task<GetCarResponse> ExecuteAsync(GetCarRequest input)
    {
        var car = await _carRepository.GetByIdAsync(input.Id);

        return car is null
            ? new("Car not found.")
            : new(car);
    }
}