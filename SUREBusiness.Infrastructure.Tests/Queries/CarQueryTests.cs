using AutoFixture;
using AutoFixture.AutoNSubstitute;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SUREBusiness.Core.Entities;
using SUREBusiness.Core.Queries;
using SUREBusiness.Infrastructure.Queries;
using Xunit;

namespace SUREBusiness.Infrastructure.Tests.Queries;

public sealed class CarQueryTests
{
    private static IFixture NewFixture()
    {
        var f = new Fixture().Customize(new AutoNSubstituteCustomization { ConfigureMembers = true });

        var options = new DbContextOptionsBuilder<SUREDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new SUREDbContext(options);
        f.Inject(context);

        f.Customize<Car>(c => c
            .With(x => x.Comments, () => new List<string>())
            .Without(x => x.BorrowedDate));

        f.Behaviors.Remove(new ThrowingRecursionBehavior());
        f.Behaviors.Add(new OmitOnRecursionBehavior());

        return f;
    }

    [Fact]
    public async Task QueryAsync_Filters_By_CurrentHolder_Correctly()
    {
        // Arrange
        var f = NewFixture();
        var context = f.Freeze<SUREDbContext>();

        var janCars = f.Build<Car>()
            .With(c => c.BorrowedTo, "Jan Jansen")
            .With(c => c.Status, CarStatus.Borrowed)
            .CreateMany(15);

        var pietCars = f.Build<Car>()
            .With(c => c.BorrowedTo, "Piet Pieters")
            .With(c => c.Status, CarStatus.Borrowed)
            .CreateMany(7);

        var availableCars = f.Build<Car>()
            .With(c => c.Status, CarStatus.Available)
            .With(c => c.BorrowedTo, (string?)null)
            .CreateMany(10);

        await context.Cars.AddRangeAsync(janCars.Concat(pietCars).Concat(availableCars));
        await context.SaveChangesAsync();

        var sut = f.Create<CarQuery>();

        var model = new CarQueryModel
        {
            CurrentHolder = "Jan Jansen",
            Page = 1,
            PageSize = 20
        };

        // Act
        var result = await sut.QueryAsync(model);

        // Assert
        result.TotalCount.Should().Be(15);
        result.Items.Should().HaveCount(15);
        result.Items.Should().AllSatisfy(c => c.BorrowedTo.Should().Be("Jan Jansen"));
    }

    [Fact]
    public async Task QueryAsync_Filters_By_Status_Correctly()
    {
        // Arrange
        var f = NewFixture();
        var context = f.Freeze<SUREDbContext>();

        var soldCars = f.Build<Car>()
            .With(c => c.Status, CarStatus.Sold)
            .CreateMany(8);

        var availableCars = f.Build<Car>()
            .With(c => c.Status, CarStatus.Available)
            .CreateMany(25);

        await context.Cars.AddRangeAsync(soldCars.Concat(availableCars));
        await context.SaveChangesAsync();

        var sut = f.Create<CarQuery>();

        var model = new CarQueryModel
        {
            Status = CarStatus.Sold,
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await sut.QueryAsync(model);

        // Assert
        result.TotalCount.Should().Be(8);
        result.Items.Should().HaveCount(8);
        result.Items.Should().AllSatisfy(c => c.Status.Should().Be(CarStatus.Sold));
    }

    [Fact]
    public async Task QueryAsync_Applies_Pagination_Correctly()
    {
        // Arrange
        var f = NewFixture();
        var context = f.Freeze<SUREDbContext>();

        var cars = f.Build<Car>()
            .With(c => c.LicensePlate, () => f.Create<string>().Substring(0, 8)) // korte unieke plaat
            .CreateMany(50)
            .OrderBy(c => c.LicensePlate)
            .ToList();

        await context.Cars.AddRangeAsync(cars);
        await context.SaveChangesAsync();

        var sut = f.Create<CarQuery>();

        var model = new CarQueryModel
        {
            Page = 2,
            PageSize = 10
        };

        // Act
        var result = await sut.QueryAsync(model);

        // Assert
        result.TotalCount.Should().Be(50);
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(10);
        result.Items.Should().HaveCount(10);

        // Check of het echt de 11e t/m 20e zijn (na sortering op LicensePlate)
        var expected = cars.Skip(10).Take(10).Select(c => c.Id).ToHashSet();
        result.Items.Select(c => c.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task QueryAsync_Returns_All_When_No_Filters_Applied()
    {
        // Arrange
        var f = NewFixture();
        var context = f.Freeze<SUREDbContext>();

        var cars = f.CreateMany<Car>(35).ToList();

        await context.Cars.AddRangeAsync(cars);
        await context.SaveChangesAsync();

        var sut = f.Create<CarQuery>();

        var model = new CarQueryModel
        {
            Page = 1,
            PageSize = 100
        };

        // Act
        var result = await sut.QueryAsync(model);

        // Assert
        result.TotalCount.Should().Be(35);
        result.Items.Should().HaveCount(35);
    }

    [Fact]
    public async Task QueryAsync_Orders_By_LicensePlate_Consistently()
    {
        // Arrange
        var f = NewFixture();
        var context = f.Freeze<SUREDbContext>();

        var cars = new[]
        {
            f.Build<Car>().With(c => c.LicensePlate, "ZZ-99-ZZ").Create(),
            f.Build<Car>().With(c => c.LicensePlate, "AA-11-AA").Create(),
            f.Build<Car>().With(c => c.LicensePlate, "MM-55-MM").Create()
        };

        await context.Cars.AddRangeAsync(cars);
        await context.SaveChangesAsync();

        var sut = f.Create<CarQuery>();

        var model = new CarQueryModel { Page = 1, PageSize = 10 };

        // Act
        var result = await sut.QueryAsync(model);

        // Assert
        result.Items.Select(c => c.LicensePlate)
              .Should()
              .ContainInOrder("AA-11-AA", "MM-55-MM", "ZZ-99-ZZ");
    }
}