using AutoFixture;
using AutoFixture.AutoNSubstitute;
using FluentAssertions;
using SUREBusiness.Api.Profiles;
using SUREBusiness.Core.Entities;
using Xunit;

namespace SUREBusiness.Api.Tests.Profiles;

public sealed class CarMappingProfileTests
{
    private static IFixture NewFixture()
    {
        var f = new Fixture().Customize(new AutoNSubstituteCustomization { ConfigureMembers = true });

        f.Customize<Car>(c => c
            .With(x => x.Comments, () => f.CreateMany<string>(3).ToList()));

        f.Behaviors.Remove(new ThrowingRecursionBehavior());
        f.Behaviors.Add(new OmitOnRecursionBehavior());

        return f;
    }

    [Fact]
    public void ToDto_Maps_All_Fields_Correctly_Including_Dutch_Status()
    {
        // Arrange
        var f = NewFixture();
        var car = f.Build<Car>()
                   .With(c => c.Status, CarStatus.Borrowed)
                   .Create();

        // Act
        var dto = car.ToDto();

        // Assert
        dto.Id.Should().Be(car.Id);
        dto.LicensePlate.Should().Be(car.LicensePlate);
        dto.Color.Should().Be(car.Color);
        dto.BuildYear.Should().Be(car.BuildYear);
        dto.BorrowedTo.Should().Be(car.BorrowedTo);
        dto.Status.Should().Be("Uitgeleend");
        dto.Remarks.Should().BeEquivalentTo(car.Comments);
    }


    [Xunit.Theory]
    [InlineData(CarStatus.Available, "Beschikbaar")]
    [InlineData(CarStatus.Borrowed, "Uitgeleend")]
    [InlineData(CarStatus.InMaintenance, "In reparatie")]
    [InlineData(CarStatus.Sold, "Verkocht")]
    [InlineData(CarStatus.Ordered, "In bestelling")]
    public void Status_Maps_To_Correct_Dutch_String(CarStatus status, string expected)
    {
        // Arrange
        var f = NewFixture();
        var car = f.Build<Car>().With(c => c.Status, status).Create();

        // Act
        var dto = car.ToDto();

        // Assert
        dto.Status.Should().Be(expected);
    }
}

