using SUREBusiness.Core.Common;
using SUREBusiness.Core.Entities;
using SUREBusiness.Core.Interfaces;

namespace SUREBusiness.Core.UseCases.Cars.Add;

public sealed class AddCarUseCase(
    IRepository<Car> carRepository,
    ILicensePlateValidator licensePlateValidator) : IAddCarUseCase
{
    private readonly IRepository<Car> _carRepository = carRepository;
    private readonly ILicensePlateValidator _licensePlateValidator = licensePlateValidator;

    public async Task<AddCarResponse> ExecuteAsync(AddCarRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.LicensePlate))
            return new("License plate is required.");

        if (string.IsNullOrWhiteSpace(input.Color))
            return new("Color is required.");

        if (input.BuildYear < 1800 || input.BuildYear > DateTime.UtcNow.Year + 1)
            return new("Build year is not plausible.");

        var isValid = await _licensePlateValidator.IsValidAsync(input.LicensePlate);
        if (!isValid)
            return new("License plate could not be validated with RDW.");

        var car = new Car
        {
            LicensePlate = input.LicensePlate,
            Color = input.Color,
            BuildYear = input.BuildYear,
            Status = CarStatus.Available,
            Comments = input.Remarks ?? []
        };

        await _carRepository.AddAsync(car);

        return new(car);
    }
}

