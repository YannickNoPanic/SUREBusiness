using AutoFixture;
using AutoFixture.AutoNSubstitute;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NSubstitute;
using SUREBusiness.Api.Controllers;
using SUREBusiness.Api.Dtos;
using SUREBusiness.Api.Profiles;
using SUREBusiness.Core.Common;
using SUREBusiness.Core.Entities;
using SUREBusiness.Core.UseCases.Cars.Add;
using SUREBusiness.Core.UseCases.Cars.Get;
using SUREBusiness.Core.UseCases.Cars.Search;
using SUREBusiness.Core.UseCases.Cars.Update;
using Xunit;

namespace SUREBusiness.Api.Tests.Controllers;

public sealed class CarsControllerTests
{
    private static IFixture NewFixture()
    {
        var f = new Fixture().Customize(new AutoNSubstituteCustomization { ConfigureMembers = true });
        f.Customize<BindingInfo>(c => c.OmitAutoProperties());
        f.Register<HttpContext>(() => new DefaultHttpContext());

        f.Behaviors.Remove(new ThrowingRecursionBehavior());
        f.Behaviors.Add(new OmitOnRecursionBehavior());

        return f;
    }

    [Fact]
    public async Task Get_ById_Returns_CarDto_When_Car_Exists()
    {
        // Arrange
        var f = NewFixture();
        var useCase = f.Freeze<IGetCarUseCase>();
        var carId = f.Create<int>();
        var car = f.Create<Car>();

        useCase.ExecuteAsync(Arg.Is<GetCarRequest>(r => r.Id == carId))
               .Returns(new GetCarResponse(car));

        var sut = f.Create<CarsController>();

        // Act
        var response = await sut.Get(useCase, carId);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>();
        var okResult = response.Result.As<OkObjectResult>();
        okResult.Value.Should().BeEquivalentTo(car.ToDto());

        await useCase.Received(1).ExecuteAsync(Arg.Is<GetCarRequest>(r => r.Id == carId));
    }

    [Fact]
    public async Task Get_ById_Returns_NotFound_When_Car_Does_Not_Exist()
    {
        // Arrange
        var f = NewFixture();
        var useCase = f.Freeze<IGetCarUseCase>();
        var carId = f.Create<int>();

        useCase.ExecuteAsync(Arg.Any<GetCarRequest>())
               .Returns(new GetCarResponse("Auto niet gevonden."));

        var sut = f.Create<CarsController>();

        // Act
        var response = await sut.Get(useCase, carId);

        // Assert
        response.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Search_Returns_Paged_CarDtos_With_Filters()
    {
        // Arrange
        var f = NewFixture();
        var useCase = f.Freeze<ISearchCarsUseCase>();

        var cars = f.CreateMany<Car>(15).ToList();
        var paged = new PagedResult<Car>() { Items = cars.Take(10).ToList().AsReadOnly(), PageSize = 15, Page = 1, TotalCount = 10 };

        useCase.ExecuteAsync(Arg.Is<SearchCarsRequest>(r =>
                r.Status == CarStatus.Available &&
                r.Page == 1 &&
                r.PageSize == 25))
               .Returns(new SearchCarsResponse(paged));

        var sut = f.Create<CarsController>();

        // Act
        var response = await sut.Search(
            useCase,
            currentHolder: null,
            status: CarStatus.Available,
            page: 1,
            pageSize: 25);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>();
        var okResult = response.Result.As<OkObjectResult>();

        await useCase.Received(1).ExecuteAsync(Arg.Is<SearchCarsRequest>(r =>
            r.Status == CarStatus.Available &&
            r.Page == 1 &&
            r.PageSize == 25));
    }

    [Fact]
    public async Task Create_Returns_CreatedAtAction_With_CarDto()
    {
        // Arrange
        var f = NewFixture();
        var useCase = f.Freeze<IAddCarUseCase>();
        var createDto = f.Create<CreateCarDto>();
        var createdCar = f.Create<Car>();

        useCase.ExecuteAsync(Arg.Any<AddCarRequest>())
               .Returns(new AddCarResponse(createdCar));

        var sut = f.Create<CarsController>();

        // Act
        var response = await sut.Create(useCase, createDto);

        // Assert
        response.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = response.As<CreatedAtActionResult>();
        createdResult.ActionName.Should().Be(nameof(CarsController.Get));
        createdResult.RouteValues!["id"].Should().Be(createdCar.Id);

        var returnedDto = createdResult.Value.As<CarDto>();
        returnedDto.Should().BeEquivalentTo(createdCar.ToDto());
    }

    [Fact]
    public async Task Update_Returns_NoContent_On_Success()
    {
        // Arrange
        var f = NewFixture();
        var useCase = f.Freeze<IUpdateCarUseCase>();
        var updateDto = f.Create<UpdateCarDto>();
        var carId = f.Create<int>();
        var updatedCar = f.Create<Car>();

        useCase.ExecuteAsync(Arg.Any<UpdateCarRequest>())
               .Returns(new UpdateCarResponse(updatedCar));

        var sut = f.Create<CarsController>();

        // Act
        var response = await sut.Update(useCase, carId, updateDto);

        // Assert
        response.Should().BeOfType<NoContentResult>();
    }
}
