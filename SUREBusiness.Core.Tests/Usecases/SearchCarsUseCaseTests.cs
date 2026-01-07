
using AutoFixture;
using FluentAssertions;
using NSubstitute;
using SUREBusiness.Core.Common;
using SUREBusiness.Core.Entities;
using SUREBusiness.Core.Queries;
using SUREBusiness.Core.UseCases.Cars.Search;
using Xunit;

namespace SUREBusiness.Core.Tests.Usecases;
public class SearchCarsUseCaseTests
{
    private static IFixture NewFixture() => TestFixture.NewFixture();

    [Fact]
    public async Task Should_Return_Filtered_And_Paged_Cars()
    {
        // Arrange
        var f = NewFixture();

        var availableCars = f.Build<Car>()
            .With(c => c.Status, CarStatus.Available)
            .CreateMany(15);

        var borrowedCars = f.Build<Car>()
            .With(c => c.Status, CarStatus.Borrowed)
            .With(c => c.BorrowedTo, "Jan Jansen")
            .CreateMany(10);

        var allCars = availableCars.Concat(borrowedCars).ToList();

        var pagedResult = new PagedResult<Car>()
        {
            Items = availableCars.Take(10).ToList().AsReadOnly(),
            TotalCount = 15,
            Page = 1,
            PageSize = 10
        };

        var carQuery = f.Freeze<ICarQuery>();
        carQuery.QueryAsync(Arg.Any<CarQueryModel>()).Returns(pagedResult);


        var request = new SearchCarsRequest(
            CurrentHolder: null,
            Status: CarStatus.Available,
            Page: 1,
            PageSize: 10);

        var sut = f.Create<SearchCarsUseCase>();

        // Act
        var result = await sut.ExecuteAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Result!.Items.Should().HaveCount(10);
        result.Result.TotalCount.Should().Be(15);
        result.Result.Items.All(c => c.Status == CarStatus.Available).Should().BeTrue();
    }
}

