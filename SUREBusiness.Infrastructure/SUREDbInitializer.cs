using Microsoft.EntityFrameworkCore;
using SUREBusiness.Core.Entities;

namespace SUREBusiness.Infrastructure;

public static class SUREDbInitializer
{
    public static async Task InitializeAsync(SUREDbContext db, CancellationToken ct = default)
    {
        await db.Database.MigrateAsync(ct);

        if (await db.Cars.AnyAsync(ct))
            return; 

        var random = new Random();
        var colors = new[] { "Rood", "Blauw", "Zwart", "Wit", "Grijs", "Zilver", "Groen" };
        var statuses = Enum.GetValues<CarStatus>();

        var cars = new List<Car>();

        for (int i = 0; i < 100; i++)
        {
            var year = random.Next(2000, DateTime.UtcNow.Year + 1);
            var status = (CarStatus)statuses.GetValue(random.Next(statuses.Length))!;

            string plate;
            do
            {
                plate = $"{RandomChar()}{RandomChar()}-{random.Next(10, 100):00}-{RandomChar()}{random.Next(10, 100)}";
            } while (cars.Any(c => c.LicensePlate == plate));

            cars.Add(new Car
            {
                LicensePlate = plate.ToUpper(),
                Color = colors[random.Next(colors.Length)],
                BuildYear = year,
                Status = status,
                BorrowedTo = status == CarStatus.Borrowed ? FakeNames[random.Next(FakeNames.Length)] : null,
                BorrowedDate = status == CarStatus.Borrowed ? DateTimeOffset.UtcNow.AddDays(-random.Next(1, 30)) : null,
                Comments = status == CarStatus.InMaintenance ? ["In onderhoud ivm remmen"] : new List<string>()
            });
        }

        db.Cars.AddRange(cars);
        await db.SaveChangesAsync(ct);
    }
    private static readonly string[] FakeNames =
    [
        "Jan Jansen", "Piet Pieters", "Kees de Vries", "Anna Bakker", "Lisa Visser",
        "Mark van Dijk", "Sophie Mulder", "Tom Hoekstra", "Emma Smit", "Luuk Boer"
    ];

    private static char RandomChar() => (char)('A' + new Random().Next(26));
}