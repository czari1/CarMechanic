using Cars.Application.Common;
using Cars.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cars.Integration.Application.Tests.Base;

[Collection("Sequential")]
public class ApplicationTestsBase
{
    protected ApplicationTestsBase()
    {
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddJsonFile("appsettings.json");
        Configuration = configurationBuilder.Build();

        var services = new ServiceCollection();
        services.AddSingleton(Configuration);

        // Register DbContext
        services.AddDbContext<CarDbContext>(options =>
            options.UseSqlServer(
                Configuration.GetConnectionString("CarsConnectionString"),
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "Cars")));

        // Register ICarContext
        services.AddScoped<ICarContext>(sp => sp.GetRequiredService<CarDbContext>());

        // Register MediatR and scan for handlers in the Application assembly
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Cars.Application.Services.AddService.AddServiceCommand).Assembly);
        });

        RegisterServices(services);
        ServiceProvider = services.BuildServiceProvider();
    }

    protected CancellationToken CancellationToken => CancellationToken.None;
    protected IConfiguration Configuration { get; }
    protected IServiceProvider ServiceProvider { get; }

    protected virtual IServiceCollection RegisterServices(IServiceCollection services)
    {
        return services;
    }

    protected TService GetRequired<TService>(IServiceScope serviceScope)
        where TService : class
    {
        return serviceScope.ServiceProvider.GetRequiredService<TService>();
    }
}