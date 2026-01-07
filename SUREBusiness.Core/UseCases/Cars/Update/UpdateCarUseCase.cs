using SUREBusiness.Core.Common;
using SUREBusiness.Core.Entities;

namespace SUREBusiness.Core.UseCases.Cars.Update;

public sealed class UpdateCarUseCase(IRepository<Car> carRepository) : IUpdateCarUseCase
{
    public async Task<UpdateCarResponse> ExecuteAsync(UpdateCarRequest request)
    {
        var car = await carRepository.GetByIdAsync(request.Id);
        if (car is null)
            return new("Auto niet gevonden.");

        // Status wijzigen regels
        if (request.Status.HasValue && request.Status != car.Status)
        {
            if (car.Status == CarStatus.Sold)
                return new("Status van een verkochte auto mag niet meer gewijzigd worden.");

            if (request.Status == CarStatus.Sold)
            {
                // Verkocht maken: mag altijd (behalve als al verkocht)
                car.Status = CarStatus.Sold;
                car.BorrowedTo = null;
                car.BorrowedDate = null;
            }
            else
            {
                car.Status = request.Status.Value;
            }
        }

        // Uitlenen regels
        if (!string.IsNullOrWhiteSpace(request.BorrowedTo))
        {
            if (car.Status != CarStatus.Available && car.Status != CarStatus.Borrowed)
                return new("Auto kan alleen uitgeleend worden als deze beschikbaar is.");

            car.BorrowedTo = request.BorrowedTo;
            car.BorrowedDate = DateTimeOffset.UtcNow;
            car.Status = CarStatus.Borrowed;
        }
        else if (string.IsNullOrWhiteSpace(request.BorrowedTo) && !string.IsNullOrWhiteSpace(car.BorrowedTo))
        {
            // Terugbrengen
            car.BorrowedTo = null;
            car.BorrowedDate = null;
            car.Status = CarStatus.Available;
        }

        // Overige velden
        if (!string.IsNullOrWhiteSpace(request.Color))
            car.Color = request.Color;

        if (request.BuildYear.HasValue)
            car.BuildYear = request.BuildYear.Value;

        if (request.RemarksToAdd?.Count > 0)
        {
            car.Comments.AddRange(request.RemarksToAdd);
        }

        await carRepository.UpdateAsync(car);

        return new(car);
    }
}