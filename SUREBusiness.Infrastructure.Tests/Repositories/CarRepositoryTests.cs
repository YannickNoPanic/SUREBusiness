using AutoFixture;
using AutoFixture.AutoNSubstitute;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SUREBusiness.Core.Entities;
using SUREBusiness.Infrastructure.Repositories;
using Xunit;

namespace SUREBusiness.Infrastructure.Tests.Repositories;

public sealed class CarRepositoryTests
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
            .With(x => x.Comments, () => new List<string>()));

        f.Behaviors.Remove(new ThrowingRecursionBehavior());
        f.Behaviors.Add(new OmitOnRecursionBehavior());

        return f;
    }

    [Fact]
    public async Task AddAsync_Adds_Car_To_Database()
    {
        // Arrange
        var f = NewFixture();
        var context = f.Freeze<SUREDbContext>();
        var repository = new BaseRepository<Car>(context); 
        var car = f.Create<Car>();

        // Act
        await repository.AddAsync(car);
        await context.SaveChangesAsync();

        // Assert
        var saved = await context.Cars.FindAsync(car.Id);
        saved.Should().BeEquivalentTo(car);
    }

    [Fact]
    public async Task GetByIdAsync_Returns_Car_When_Exists()
    {
        // Arrange
        var f = NewFixture();
        var context = f.Freeze<SUREDbContext>();
        var repository = new BaseRepository<Car>(context);
        var car = f.Create<Car>();
        await context.Cars.AddAsync(car);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(car.Id);

        // Assert
        result.Should().BeEquivalentTo(car);
    }    
}