using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SUREBusiness.Core.Entities;
using System.Text.Json;

namespace SUREBusiness.Infrastructure.Configurations;

public sealed class CarConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.ToTable("Cars");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.LicensePlate)
            .IsRequired()
            .HasMaxLength(16);

        builder.HasIndex(c => c.LicensePlate).IsUnique();

        builder.Property(c => c.Color)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(c => c.BuildYear)
            .IsRequired();

        builder.Property(c => c.Status).HasConversion<string>()
            .IsRequired();

        builder.Property(c => c.Comments).HasConversion(
            v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
            v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions()) ?? new());

        builder.Property(c => c.BorrowedTo)
            .HasMaxLength(256);

    }
}
