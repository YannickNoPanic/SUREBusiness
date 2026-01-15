using Microsoft.EntityFrameworkCore;
using SUREBusiness.Infrastructure;
using SUREBusiness.Core;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Laag-specifieke registraties
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment.IsDevelopment());
builder.Services.AddCore(builder.Configuration);

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

// === DATABASE INITIALISATIE – SUPER BELANGRIJK ===
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SUREDbContext>();

    // Pas migrations toe (maakt DB aan als die niet bestaat)
    await db.Database.MigrateAsync();

    // Seed 100 dummy auto's als de tabel leeg is
    await SUREDbInitializer.InitializeAsync(db);
}

app.Run();