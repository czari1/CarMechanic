using Cars.Application.Common;
using Cars.Application.Services.UpdateService;
using Cars.Integration.Application.Builder;
using Cars.Integration.Application.Tests.Base;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Cars.Integration.Application.Tests.Services.UpdateService;

[Trait("Category", "IntegrationTests")]
public class UpdateServiceCommandHandlerTests : ApplicationTestsBase
{
    [Fact]
    public async Task Handle_Should_Update_Service_Successfully()
    {
        int? serviceId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<UpdateServiceCommand>>();

            // ARRANGE
            var service = await new ServiceBuilder(ctx)
                .WithDefaults(serviceName: "Oil Change", serviceDescription: "Standard oil change", price: 150.00m)
                .Build();

            serviceId = service.Id;

            var cmd = new UpdateServiceCommand(
                serviceId.Value,
                "Premium Oil Change",
                "Synthetic oil change service",
                250.00m);

            // ACT
            await handler.Handle(cmd, CancellationToken);

            // ASSERT
            using var verifyScope = ServiceProvider.CreateScope();
            var vctx = verifyScope.ServiceProvider.GetRequiredService<ICarContext>();

            var updatedService = await vctx.Services
                .FirstOrDefaultAsync(s => s.Id == serviceId.Value, CancellationToken);

            updatedService.ShouldNotBeNull();
            updatedService.ServiceName.ShouldBe("Premium Oil Change");
            updatedService.ServiceDescription.ShouldBe("Synthetic oil change service");
            updatedService.Price.ShouldBe(250.00m);
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
    public async Task Handle_Should_Update_Only_ServiceName()
    {
        int? serviceId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<UpdateServiceCommand>>();

            // ARRANGE
            var service = await new ServiceBuilder(ctx)
                .WithDefaults(serviceName: "Oil Change", serviceDescription: "Standard service", price: 150.00m)
                .Build();

            serviceId = service.Id;

            var cmd = new UpdateServiceCommand(
                serviceId.Value,
                "Quick Oil Change",
                "",
                150.00m);

            // ACT
            await handler.Handle(cmd, CancellationToken);

            // ASSERT
            using var verifyScope = ServiceProvider.CreateScope();
            var vctx = verifyScope.ServiceProvider.GetRequiredService<ICarContext>();

            var updatedService = await vctx.Services
                .FirstOrDefaultAsync(s => s.Id == serviceId.Value, CancellationToken);

            updatedService.ShouldNotBeNull();
            updatedService.ServiceName.ShouldBe("Quick Oil Change");
            updatedService.ServiceDescription.ShouldBe("Standard service"); // Unchanged
            updatedService.Price.ShouldBe(150.00m);
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
    public async Task Handle_Should_Update_Price_To_Zero()
    {
        int? serviceId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<UpdateServiceCommand>>();

            // ARRANGE
            var service = await new ServiceBuilder(ctx)
                .WithDefaults(price: 150.00m)
                .Build();

            serviceId = service.Id;

            var cmd = new UpdateServiceCommand(
                serviceId.Value,
                "Free Inspection",
                "Complimentary service",
                0m);

            // ACT
            await handler.Handle(cmd, CancellationToken);

            // ASSERT
            using var verifyScope = ServiceProvider.CreateScope();
            var vctx = verifyScope.ServiceProvider.GetRequiredService<ICarContext>();

            var updatedService = await vctx.Services
                .FirstOrDefaultAsync(s => s.Id == serviceId.Value, CancellationToken);

            updatedService.ShouldNotBeNull();
            updatedService.Price.ShouldBe(0m);
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
    public async Task Handle_Should_Throw_When_Service_Does_Not_Exist()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;
        var handler = sp.GetRequiredService<IRequestHandler<UpdateServiceCommand>>();

        // ARRANGE: Non-existent service
        var cmd = new UpdateServiceCommand(
            999999,
            "Service",
            "Description",
            100m);

        // ACT & ASSERT
        await Should.ThrowAsync<KeyNotFoundException>(
            async () => await handler.Handle(cmd, CancellationToken));
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_ServiceName_Exceeds_Max_Length()
    {
        int? serviceId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<UpdateServiceCommand>>();

            // ARRANGE
            var service = await new ServiceBuilder(ctx)
                .WithDefaults()
                .Build();

            serviceId = service.Id;

            var longName = new string('A', 51); // Exceeds 50 characters

            var cmd = new UpdateServiceCommand(
                serviceId.Value,
                longName,
                "Description",
                100m);

            // ACT & ASSERT
            await Should.ThrowAsync<ValidationException>(
                async () => await handler.Handle(cmd, CancellationToken));
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
    public async Task Handle_Should_Throw_ValidationException_When_ServiceDescription_Exceeds_Max_Length()
    {
        int? serviceId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<UpdateServiceCommand>>();

            // ARRANGE
            var service = await new ServiceBuilder(ctx)
                .WithDefaults()
                .Build();

            serviceId = service.Id;

            var longDescription = new string('B', 201); // Exceeds 200 characters

            var cmd = new UpdateServiceCommand(
                serviceId.Value,
                "Service Name",
                longDescription,
                100m);

            // ACT & ASSERT
            await Should.ThrowAsync<ValidationException>(
                async () => await handler.Handle(cmd, CancellationToken));
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
    public async Task Handle_Should_Throw_ValidationException_When_Price_Is_Negative()
    {
        int? serviceId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<UpdateServiceCommand>>();

            // ARRANGE
            var service = await new ServiceBuilder(ctx)
                .WithDefaults()
                .Build();

            serviceId = service.Id;

            var cmd = new UpdateServiceCommand(
                serviceId.Value,
                "Service Name",
                "Description",
                -50m);

            // ACT & ASSERT
            await Should.ThrowAsync<ValidationException>(
                async () => await handler.Handle(cmd, CancellationToken));
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
    public async Task Handle_Should_Not_Change_ServiceDate()
    {
        int? serviceId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<UpdateServiceCommand>>();

            // ARRANGE
            var service = await new ServiceBuilder(ctx)
                .WithDefaults()
                .Build();

            serviceId = service.Id;
            var originalServiceDate = service.ServiceDate;

            await Task.Delay(100);

            var cmd = new UpdateServiceCommand(
                serviceId.Value,
                "Updated Service",
                "Updated Description",
                200m);

            // ACT
            await handler.Handle(cmd, CancellationToken);

            // ASSERT
            using var verifyScope = ServiceProvider.CreateScope();
            var vctx = verifyScope.ServiceProvider.GetRequiredService<ICarContext>();

            var updatedService = await vctx.Services
                .FirstOrDefaultAsync(s => s.Id == serviceId.Value, CancellationToken);

            updatedService.ShouldNotBeNull();
            updatedService.ServiceDate.ShouldBe(originalServiceDate);
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