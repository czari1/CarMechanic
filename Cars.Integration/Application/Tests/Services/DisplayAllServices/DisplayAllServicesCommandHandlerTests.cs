using Cars.Application.Common;
using Cars.Application.Services.DisplayAllServices;
using Cars.Application.Services.Models;
using Cars.Integration.Application.Builder;
using Cars.Integration.Application.Tests.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Cars.Integration.Application.Tests.Services.DisplayAllServices;

[Trait("Category", "IntegrationTests")]
public class DisplayAllServicesCommandHandlerTests : ApplicationTestsBase
{
    [Fact]
    public async Task Handle_Should_Return_All_Services()
    {
        int? serviceId1 = null;
        int? serviceId2 = null;
        int? serviceId3 = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<DisplayAllServicesQuery, IEnumerable<ServiceListDto>>>();

            var initialCount = await ctx.Services.CountAsync(CancellationToken);

            var service1 = await new ServiceBuilder(ctx)
                .WithDefaults(
                    serviceName: "Oil Change",
                    serviceDescription: "Standard oil change",
                    price: 150.00m)
                .Build(CancellationToken);

            var service2 = await new ServiceBuilder(ctx)
                .WithDefaults(
                    serviceName: "Tire Rotation",
                    serviceDescription: "4-wheel tire rotation",
                    price: 75.00m)
                .Build(CancellationToken);

            var service3 = await new ServiceBuilder(ctx)
                .WithDefaults(
                    serviceName: "Brake Service",
                    serviceDescription: "Full brake inspection",
                    price: 200.00m)
                .Build(CancellationToken);

            serviceId1 = service1.Id;
            serviceId2 = service2.Id;
            serviceId3 = service3.Id;

            var cmd = new DisplayAllServicesQuery();

            var result = await handler.Handle(cmd, CancellationToken);

            var servicesList = result.ToList();
            servicesList.Count.ShouldBe(initialCount + 3);
            servicesList.ShouldContain(s => s.ServiceName == "Oil Change" && s.Price == 150.00m);
            servicesList.ShouldContain(s => s.ServiceName == "Tire Rotation" && s.Price == 75.00m);
            servicesList.ShouldContain(s => s.ServiceName == "Brake Service" && s.Price == 200.00m);
        }
        finally
        {
            using var scope = ServiceProvider.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<ICarContext>();

            if (serviceId1.HasValue)
                await ctx.Services.Where(x => x.Id == serviceId1.Value).ExecuteDeleteAsync(CancellationToken);
            if (serviceId2.HasValue)
                await ctx.Services.Where(x => x.Id == serviceId2.Value).ExecuteDeleteAsync(CancellationToken);
            if (serviceId3.HasValue)
                await ctx.Services.Where(x => x.Id == serviceId3.Value).ExecuteDeleteAsync(CancellationToken);
        }
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_List_When_No_Additional_Services_Exist()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;
        var ctx = sp.GetRequiredService<ICarContext>();
        var handler = sp.GetRequiredService<IRequestHandler<DisplayAllServicesQuery, IEnumerable<ServiceListDto>>>();

        var existingCount = await ctx.Services.CountAsync(CancellationToken);

        var cmd = new DisplayAllServicesQuery();

        var result = await handler.Handle(cmd, CancellationToken);

        result.Count().ShouldBe(existingCount);
    }

    [Fact]
    public async Task Handle_Should_Return_Services_With_All_Required_Properties()
    {
        int? serviceId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<DisplayAllServicesQuery, IEnumerable<ServiceListDto>>>();

            var service = await new ServiceBuilder(ctx)
                .WithDefaults(
                    serviceName: "Test Service",
                    serviceDescription: "Test Description",
                    price: 100.00m)
                .Build(CancellationToken);

            serviceId = service.Id;

            var cmd = new DisplayAllServicesQuery();

            var result = await handler.Handle(cmd, CancellationToken);

            var testService = result.FirstOrDefault(s => s.ServiceId == serviceId.Value);

            testService.ShouldNotBeNull();
            testService.ServiceId.ShouldBeGreaterThan(0);
            testService.ServiceName.ShouldNotBeNullOrEmpty();
            testService.ServiceDescription.ShouldNotBeNullOrEmpty();
            testService.Price.ShouldBeGreaterThanOrEqualTo(0);
            testService.ServiceDate.ShouldBeOfType<DateTime>();
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
    public async Task Handle_Should_Return_Services_Ordered_Or_Unordered_Consistently()
    {
        int? serviceId1 = null;
        int? serviceId2 = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<DisplayAllServicesQuery, IEnumerable<ServiceListDto>>>();

            var service1 = await new ServiceBuilder(ctx)
                .WithDefaults(serviceName: "A Service", price: 100.00m)
                .Build(CancellationToken);

            var service2 = await new ServiceBuilder(ctx)
                .WithDefaults(serviceName: "B Service", price: 200.00m)
                .Build(CancellationToken);

            serviceId1 = service1.Id;
            serviceId2 = service2.Id;

            var cmd = new DisplayAllServicesQuery();

            var result1 = await handler.Handle(cmd, CancellationToken);
            var result2 = await handler.Handle(cmd, CancellationToken);

            var list1 = result1.Where(s => s.ServiceId == serviceId1.Value || s.ServiceId == serviceId2.Value).ToList();
            var list2 = result2.Where(s => s.ServiceId == serviceId1.Value || s.ServiceId == serviceId2.Value).ToList();

            list1.Count.ShouldBe(list2.Count);
            list1.Select(s => s.ServiceId).ShouldBe(list2.Select(s => s.ServiceId));
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
    public async Task Handle_Should_Return_Services_With_Different_Prices()
    {
        int? serviceId1 = null;
        int? serviceId2 = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<DisplayAllServicesQuery, IEnumerable<ServiceListDto>>>();

            var service1 = await new ServiceBuilder(ctx)
                .WithDefaults(serviceName: "Cheap Service", price: 50.00m)
                .Build(CancellationToken);

            var service2 = await new ServiceBuilder(ctx)
                .WithDefaults(serviceName: "Expensive Service", price: 1500.00m)
                .Build(CancellationToken);

            serviceId1 = service1.Id;
            serviceId2 = service2.Id;

            var cmd = new DisplayAllServicesQuery();

            var result = await handler.Handle(cmd, CancellationToken);

            var servicesList = result.ToList();
            servicesList.ShouldContain(s => s.ServiceName == "Cheap Service" && s.Price == 50.00m);
            servicesList.ShouldContain(s => s.ServiceName == "Expensive Service" && s.Price == 1500.00m);
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
}