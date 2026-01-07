using AutoFixture;
using FluentAssertions;
using NSubstitute;
using SUREBusiness.Core.Common;
using SUREBusiness.Core.Entities;
using SUREBusiness.Core.Interfaces;
using SUREBusiness.Core.UseCases.Cars.Add;
using Xunit;

namespace SUREBusiness.Core.Tests.Usecases;

public class AddCarUseCaseTests
{
    private static IFixture NewFixture() => TestFixture.NewFixture();

    [Fact]
    public async Task Should_Add_Car_When_LicensePlate_Is_Valid_According_To_RDW()
    {
        // Arrange
        var f = NewFixture();
        var request = f.Create<AddCarRequest>();
        var repo = f.Freeze<IRepository<Car>>();
        var validator = f.Freeze<ILicensePlateValidator>();

        validator.IsValidAsync(request.LicensePlate).Returns(true);

        var sut = f.Create<AddCarUseCase>();

        // Act
        var result = await sut.ExecuteAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Result.Should().NotBeNull();

        await repo.Received(1).AddAsync(Arg.Any<Car>());
    }

    [Fact]
    public async Task Should_Return_Error_When_LicensePlate_Not_Valid_According_To_RDW()
    {
        // Arrange
        var f = NewFixture();
        var request = f.Create<AddCarRequest>();
        var validator = f.Freeze<ILicensePlateValidator>();

        validator.IsValidAsync(request.LicensePlate).Returns(false);

        var sut = f.Create<AddCarUseCase>();

        // Act
        var result = await sut.ExecuteAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessages.Should().Contain("License plate could not be validated with RDW.");
    }
}

