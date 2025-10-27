using Cars.Application.Common;
using Cars.Application.Services.GetServiceById;
using Cars.Application.Services.Models;
using Cars.Integration.Application.Builder;
using Cars.Integration.Application.Tests.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Cars.Integration.Application.Tests.Services.GetServiceById;

[Trait("Category", "IntegrationTests")]
public class GetServiceByIdCommandHandlerTests : ApplicationTestsBase
{
    [Fact]
    public async Task Handle_Should_Return_Service_When_Found()
    {
        int? serviceId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<GetServiceByIdCommand, ServiceDto?>>();

            var service = await new ServiceBuilder(ctx)
                .WithDefaults(
                    serviceName: "Tire Rotation",
                    serviceDescription: "Professional tire rotation service",
                    price: 75.00m)
                .Build(CancellationToken);

            serviceId = service.Id;

            var cmd = new GetServiceByIdCommand(serviceId.Value);

            var result = await handler.Handle(cmd, CancellationToken);

            result.ShouldNotBeNull();
            result.ServiceId.ShouldBe(serviceId.Value);
            result.ServiceName.ShouldBe("Tire Rotation");
            result.ServiceDescription.ShouldBe("Professional tire rotation service");
            result.Price.ShouldBe(75.00m);
            result.ServiceDate.ShouldBeOfType<DateTime>();
            result.CreatedOn.ShouldBeOfType<DateTime>();
            result.ModifiedOn.ShouldBeOfType<DateTime>();
        }
        finally
        {
            if (serviceId.HasValue)
            {
                using var scope = ServiceProvider.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<ICarContext>();
                await ctx.Services
                    .Where(x => x.Id == serviceId.Value)
                    .ExecuteDeleteAsync(CancellationToken);
            }
        }
    }

    [Fact]
    public async Task Handle_Should_Return_Null_When_Service_Not_Found()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;
        var handler = sp.GetRequiredService<IRequestHandler<GetServiceByIdCommand, ServiceDto?>>();

        var cmd = new GetServiceByIdCommand(99999);

        var result = await handler.Handle(cmd, CancellationToken);

        result.ShouldBeNull();
    }

    [Fact]
    public async Task Handle_Should_Return_Service_With_Correct_Data_Types()
    {
        int? serviceId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<GetServiceByIdCommand, ServiceDto?>>();

            var service = await new ServiceBuilder(ctx)
                .WithDefaults()
                .Build(CancellationToken);

            serviceId = service.Id;

            var cmd = new GetServiceByIdCommand(serviceId.Value);

            var result = await handler.Handle(cmd, CancellationToken);

            result.ShouldNotBeNull();
            result.ServiceId.ShouldBeOfType<int>();
            result.ServiceName.ShouldBeOfType<string>();
            result.ServiceDescription.ShouldBeOfType<string>();
            result.Price.ShouldBeOfType<decimal>();
            result.ServiceDate.ShouldBeOfType<DateTime>();
            result.CreatedOn.ShouldBeOfType<DateTime>();
            result.ModifiedOn.ShouldBeOfType<DateTime>();
        }
        finally
        {
            if (serviceId.HasValue)
            {
                using var scope = ServiceProvider.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<ICarContext>();
                await ctx.Services
                    .Where(x => x.Id == serviceId.Value)
                    .ExecuteDeleteAsync(CancellationToken);
            }
        }
    }
}