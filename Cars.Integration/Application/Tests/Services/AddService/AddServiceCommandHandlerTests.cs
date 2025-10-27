using Cars.Application.Services.AddService;
using Cars.Application.Common;
using Cars.Integration.Application.Tests.Base;
using MediatR;
using Shouldly;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cars.Integration.Application.Tests.Services.AddService;

[Trait("Category", "IntegrationTests")]
public class AddServiceCommandHandlerTests_Enhanced : ApplicationTestsBase
{
    [Fact]
    public async Task Handle_Should_Add_Service_And_Return_ServiceId()
    {
        int? serviceId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();

            var cmd = new AddServiceCommand(
                ServiceId: 0,
                ServiceName: "Wymiana",
                ServiceDescription: "Wymiana skrzyni",
                Price: 200.0m,
                ServiceDate: DateTime.UtcNow);

            var handler = sp.GetRequiredService<IRequestHandler<AddServiceCommand, int>>();

            serviceId = await handler.Handle(cmd, CancellationToken);

            var created = await ctx.Services
                .Where(x => x.Id == serviceId)
                .SingleOrDefaultAsync(CancellationToken);

            created.ShouldNotBeNull();
            created.ServiceName.ShouldBe("Wymiana");
            created.ServiceDescription.ShouldBe("Wymiana skrzyni");
            created.Price.ShouldBe(200.0m);
            created.ServiceDate.ShouldBeOfType<DateTime>();
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
    public async Task Handle_Should_Set_CreatedOn_And_ModifiedOn_Timestamps()
    {
        int? serviceId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();

            var beforeCreation = DateTime.UtcNow;

            var cmd = new AddServiceCommand(
                ServiceId: 0,
                ServiceName: "Test Service",
                ServiceDescription: "Test Description",
                Price: 100.0m,
                ServiceDate: DateTime.UtcNow);

            var handler = sp.GetRequiredService<IRequestHandler<AddServiceCommand, int>>();
            serviceId = await handler.Handle(cmd, CancellationToken);

            var afterCreation = DateTime.UtcNow;

            var created = await ctx.Services
                .Where(x => x.Id == serviceId)
                .SingleOrDefaultAsync(CancellationToken);

            created.ShouldNotBeNull();
            created.CreatedOn.ShouldBeGreaterThanOrEqualTo(beforeCreation);
            created.CreatedOn.ShouldBeLessThanOrEqualTo(afterCreation);
            created.ModifiedOn.ShouldBeGreaterThanOrEqualTo(beforeCreation);
            created.ModifiedOn.ShouldBeLessThanOrEqualTo(afterCreation);
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
    public async Task Handle_Should_Add_Service_With_Zero_Price()
    {
        int? serviceId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();

            var cmd = new AddServiceCommand(
                ServiceId: 0,
                ServiceName: "Free Inspection",
                ServiceDescription: "Complimentary vehicle inspection",
                Price: 0.0m,
                ServiceDate: DateTime.UtcNow);

            var handler = sp.GetRequiredService<IRequestHandler<AddServiceCommand, int>>();
            serviceId = await handler.Handle(cmd, CancellationToken);

            var created = await ctx.Services
                .Where(x => x.Id == serviceId)
                .SingleOrDefaultAsync(CancellationToken);

            created.ShouldNotBeNull();
            created.Price.ShouldBe(0.0m);
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
    public async Task Handle_Should_Add_Service_With_Maximum_Length_Values()
    {
        int? serviceId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();

            var maxServiceName = new string('A', 50);
            var maxServiceDescription = new string('B', 200);

            var cmd = new AddServiceCommand(
                ServiceId: 0,
                ServiceName: maxServiceName,
                ServiceDescription: maxServiceDescription,
                Price: 999.99m,
                ServiceDate: DateTime.UtcNow);

            var handler = sp.GetRequiredService<IRequestHandler<AddServiceCommand, int>>();
            serviceId = await handler.Handle(cmd, CancellationToken);

            var created = await ctx.Services
                .Where(x => x.Id == serviceId)
                .SingleOrDefaultAsync(CancellationToken);

            created.ShouldNotBeNull();
            created.ServiceName.ShouldBe(maxServiceName);
            created.ServiceDescription.ShouldBe(maxServiceDescription);
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
    public async Task Handle_Should_Throw_ValidationException_When_ServiceName_Is_Empty()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        var cmd = new AddServiceCommand(
            ServiceId: 0,
            ServiceName: "",
            ServiceDescription: "Wymiana skrzyni",
            Price: 200.0m,
            ServiceDate: DateTime.UtcNow);

        var handler = sp.GetRequiredService<IRequestHandler<AddServiceCommand, int>>();

        await Should.ThrowAsync<FluentValidation.ValidationException>(
            async () => await handler.Handle(cmd, CancellationToken));
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_ServiceName_Is_Whitespace()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        var cmd = new AddServiceCommand(
            ServiceId: 0,
            ServiceName: "   ",
            ServiceDescription: "Valid Description",
            Price: 200.0m,
            ServiceDate: DateTime.UtcNow);

        var handler = sp.GetRequiredService<IRequestHandler<AddServiceCommand, int>>();

        await Should.ThrowAsync<FluentValidation.ValidationException>(
            async () => await handler.Handle(cmd, CancellationToken));
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_ServiceDescription_Is_Empty()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        var cmd = new AddServiceCommand(
            ServiceId: 0,
            ServiceName: "Valid Name",
            ServiceDescription: "",
            Price: 200.0m,
            ServiceDate: DateTime.UtcNow);

        var handler = sp.GetRequiredService<IRequestHandler<AddServiceCommand, int>>();

        await Should.ThrowAsync<FluentValidation.ValidationException>(
            async () => await handler.Handle(cmd, CancellationToken));
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_ServiceName_Exceeds_50_Characters()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        var cmd = new AddServiceCommand(
            ServiceId: 0,
            ServiceName: new string('A', 51),
            ServiceDescription: "Valid Description",
            Price: 200.0m,
            ServiceDate: DateTime.UtcNow);

        var handler = sp.GetRequiredService<IRequestHandler<AddServiceCommand, int>>();

        await Should.ThrowAsync<FluentValidation.ValidationException>(
            async () => await handler.Handle(cmd, CancellationToken));
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_ServiceDescription_Exceeds_200_Characters()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        var cmd = new AddServiceCommand(
            ServiceId: 0,
            ServiceName: "Valid Name",
            ServiceDescription: new string('A', 201),
            Price: 200.0m,
            ServiceDate: DateTime.UtcNow);

        var handler = sp.GetRequiredService<IRequestHandler<AddServiceCommand, int>>();

        await Should.ThrowAsync<FluentValidation.ValidationException>(
            async () => await handler.Handle(cmd, CancellationToken));
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_Price_Is_Negative()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        var cmd = new AddServiceCommand(
            ServiceId: 0,
            ServiceName: "Valid Name",
            ServiceDescription: "Valid Description",
            Price: -1.0m,
            ServiceDate: DateTime.UtcNow);

        var handler = sp.GetRequiredService<IRequestHandler<AddServiceCommand, int>>();

        await Should.ThrowAsync<FluentValidation.ValidationException>(
            async () => await handler.Handle(cmd, CancellationToken));
    }

    [Fact]
    public async Task Handle_Should_Add_Multiple_Services_With_Same_Name()
    {
        int? serviceId1 = null;
        int? serviceId2 = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();

            var cmd1 = new AddServiceCommand(
                ServiceId: 0,
                ServiceName: "Oil Change",
                ServiceDescription: "Standard oil change",
                Price: 150.0m,
                ServiceDate: DateTime.UtcNow);

            var cmd2 = new AddServiceCommand(
                ServiceId: 0,
                ServiceName: "Oil Change",
                ServiceDescription: "Premium oil change",
                Price: 200.0m,
                ServiceDate: DateTime.UtcNow);

            var handler = sp.GetRequiredService<IRequestHandler<AddServiceCommand, int>>();
            serviceId1 = await handler.Handle(cmd1, CancellationToken);
            serviceId2 = await handler.Handle(cmd2, CancellationToken);

            var services = await ctx.Services
                .Where(x => x.Id == serviceId1 || x.Id == serviceId2)
                .ToListAsync(CancellationToken);

            services.Count.ShouldBe(2);
            services.All(s => s.ServiceName == "Oil Change").ShouldBeTrue();
        }
        finally
        {
            using var scope = ServiceProvider.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<ICarContext>();

            if (serviceId1.HasValue)
                await ctx.Services.Where(x => x.Id == serviceId1.Value).ExecuteDeleteAsync(CancellationToken);
            if (serviceId2.HasValue)
                await ctx.Services.Where(x => x.Id == serviceId2.Value).ExecuteDeleteAsync(CancellationToken);
        }
    }

    [Fact]
    public async Task Handle_Should_Add_Service_With_Large_Price()
    {
        int? serviceId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();

            var cmd = new AddServiceCommand(
                ServiceId: 0,
                ServiceName: "Engine Rebuild",
                ServiceDescription: "Complete engine rebuild service",
                Price: 99999.99m,
                ServiceDate: DateTime.UtcNow);

            var handler = sp.GetRequiredService<IRequestHandler<AddServiceCommand, int>>();
            serviceId = await handler.Handle(cmd, CancellationToken);

            var created = await ctx.Services
                .Where(x => x.Id == serviceId)
                .SingleOrDefaultAsync(CancellationToken);

            created.ShouldNotBeNull();
            created.Price.ShouldBe(99999.99m);
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