using Cars.Application.Common;
using Cars.Application.Services.DisplayAllServices;
using Cars.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// 💡 UPEWNIJ SIĘ, ŻE TE DWIE LINIE SĄ OBECNE
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DisplayAllServicesQuery).Assembly));

builder.Services.AddDbContext<CarDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("CarsConnectionString"),
        x => x.MigrationsHistoryTable("__EFMigrationsHistory", "Cars")));
builder.Services.AddScoped<ICarContext>(provider => provider.GetRequiredService<CarDbContext>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();