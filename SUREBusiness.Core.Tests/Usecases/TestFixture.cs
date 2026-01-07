
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using SUREBusiness.Core.Entities;
using SUREBusiness.Core.UseCases.Cars.Add;
using SUREBusiness.Core.UseCases.Cars.Update;

namespace SUREBusiness.Core.Tests.Usecases;
public static class TestFixture
{
    public static IFixture NewFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization
        {
            ConfigureMembers = true,
            GenerateDelegates = true
        });

        // Standaard waarden voor Car
        fixture.Customize<Car>(c => c
            .With(x => x.Status, CarStatus.Available)
            .With(x => x.Comments, new List<string>())
            .Without(x => x.BorrowedTo)
            .Without(x => x.BorrowedDate));

        // Requests
        fixture.Customize<AddCarRequest>(c => c
            .With(x => x.LicensePlate, () => GenerateValidLicensePlate(fixture))
            .With(x => x.Color, "Rood")
            .With(x => x.BuildYear, 2023));

        fixture.Customize<UpdateCarRequest>(c => c
            .With(x => x.Id, 1));

        // Recursion voorkomen
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        return fixture;
    }

    private static string GenerateValidLicensePlate(IFixture fixture)
    {
        var rand = new Random();
        return $"{RandomLetters(2)}{rand.Next(10, 100):00}-{RandomLetters(2)}";
    }

    private static string RandomLetters(int count)
    {
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var rand = new Random();
        return new string(Enumerable.Repeat(letters, count).Select(s => s[rand.Next(s.Length)]).ToArray());
    }
}


