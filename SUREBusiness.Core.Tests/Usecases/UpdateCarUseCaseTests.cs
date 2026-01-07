
using AutoFixture;
using FluentAssertions;
using NSubstitute;
using SUREBusiness.Core.Common;
using SUREBusiness.Core.Entities;
using SUREBusiness.Core.UseCases.Cars.Update;
using Xunit;

namespace SUREBusiness.Core.Tests.Usecases;
public class UpdateCarUseCaseTests
{
    private static IFixture NewFixture() => TestFixture.NewFixture();

    [Fact]
    public async Task Should_Lend_Car_When_Status_Is_Available()
    {
        // Arrange
        var f = NewFixture();
        var car = f.Build<Car>()
            .With(c => c.Status, CarStatus.Available)
            .Create();

        var request = f.Build<UpdateCarRequest>()
            .With(r => r.Id, car.Id)
            .With(r => r.BorrowedTo, "Jan Jansen")
            .Create();

        var repo = f.Freeze<IRepository<Car>>();
        repo.GetByIdAsync(car.Id).Returns(car);

        var sut = f.Create<UpdateCarUseCase>();

        // Act
        var result = await sut.ExecuteAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        car.Status.Should().Be(CarStatus.Borrowed);
        car.BorrowedTo.Should().Be("Jan Jansen");
        car.BorrowedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));

        await repo.Received(1).UpdateAsync(car);
    }

    [Fact]
    public async Task Should_Not_Lend_Car_When_Status_Is_Not_Available()
    {
        // Arrange
        var f = NewFixture();
        var car = f.Build<Car>()
            .With(c => c.Status, CarStatus.InMaintenance)
            .Without(c => c.BorrowedTo)
            .Create();

        var request = new UpdateCarRequest(
            Id: car.Id,
            BorrowedTo: "Jan Jansen");

        var repo = f.Freeze<IRepository<Car>>();
        repo.GetByIdAsync(car.Id).Returns(car);

        var sut = f.Create<UpdateCarUseCase>();

        // Act
        var result = await sut.ExecuteAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessages.Should().Contain("Auto kan alleen uitgeleend worden als deze beschikbaar is.");
        car.BorrowedTo.Should().BeNull(); // niet gewijzigd
    }

    [Fact]
    public async Task Should_Return_Car_When_BorrowedTo_Is_Cleared()
    {
        // Arrange
        var f = NewFixture();
        var car = f.Build<Car>()
            .With(c => c.Status, CarStatus.Borrowed)
            .With(c => c.BorrowedTo, "Jan Jansen")
            .Create();

        var request = new UpdateCarRequest(Id: car.Id, BorrowedTo: "");

        var repo = f.Freeze<IRepository<Car>>();
        repo.GetByIdAsync(car.Id).Returns(car);

        var sut = f.Create<UpdateCarUseCase>();

        // Act
        var result = await sut.ExecuteAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        car.Status.Should().Be(CarStatus.Available);
        car.BorrowedTo.Should().BeNull();
        car.BorrowedDate.Should().BeNull();
    }

    [Fact]
    public async Task Should_Not_Change_Status_When_Car_Is_Sold()
    {
        // Arrange
        var f = NewFixture();
        var car = f.Build<Car>()
            .With(c => c.Status, CarStatus.Sold)
            .Create();

        var request = new UpdateCarRequest(Id: car.Id, Status: CarStatus.Available);

        var repo = f.Freeze<IRepository<Car>>();
        repo.GetByIdAsync(car.Id).Returns(car);

        var sut = f.Create<UpdateCarUseCase>();

        // Act
        var result = await sut.ExecuteAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessages.Should().Contain("Status van een verkochte auto mag niet meer gewijzigd worden.");
        car.Status.Should().Be(CarStatus.Sold);
    }

    [Fact]
    public async Task Should_Allow_Setting_Status_To_Sold()
    {
        // Arrange
        var f = NewFixture();
        var car = f.Build<Car>()
            .With(c => c.Status, CarStatus.Available)
            .Create();

        var request = new UpdateCarRequest(Id: car.Id, Status: CarStatus.Sold);

        var repo = f.Freeze<IRepository<Car>>();
        repo.GetByIdAsync(car.Id).Returns(car);

        var sut = f.Create<UpdateCarUseCase>();

        // Act
        var result = await sut.ExecuteAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        car.Status.Should().Be(CarStatus.Sold);
        car.BorrowedTo.Should().BeNull();
        car.BorrowedDate.Should().BeNull();
    }

    [Fact]
    public async Task Should_Add_Comment_To_Car()
    {
        // Arrange
        var f = NewFixture();
        var car = f.Create<Car>();
        var newComments = new List<string> { "Remmen vervangen", "APK gedaan" };

        var request = new UpdateCarRequest(
            Id: car.Id,
            RemarksToAdd: newComments);

        var repo = f.Freeze<IRepository<Car>>();
        repo.GetByIdAsync(car.Id).Returns(car);

        var sut = f.Create<UpdateCarUseCase>();

        // Act
        var result = await sut.ExecuteAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        car.Comments.Should().Contain(newComments);
    }
}


