using Cars.Application.Clients.GetCarById;
using Cars.Application.Clients.Models;
using Cars.Application.Common;
using Cars.Integration.Application.Builder;
using Cars.Integration.Application.Tests.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Cars.Integration.Application.Tests.Client.GetCarById;
public class GetCarByIdCommandHandlerTests : ApplicationTestsBase
{
    [Fact]
    public async Task Handle_Should_Get_CarId_And_Return_Information()
    {
        int? clientId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<GetCarByIdCommand, CarDto?>>();

            var client = await new ClientBuilder(ctx)
                .WithDefaults(name: "Jan", surname: "Kowalski", phoneNumber: "123456789")
                .WithCar("Toyota", "Corolla", 2020, "VIN00000000000001")
                .Build();

            clientId = client.Id;
            var carId = client.Cars.First().Id;

            var cmd = new GetCarByIdCommand(carId);

            var result = await handler.Handle(cmd, CancellationToken);

            result.ShouldNotBeNull();
            result.CarId.ShouldBe(carId);
            result.Make.ShouldBe("Toyota");
            result.Model.ShouldBe("Corolla");
            result.Year.ShouldBe(2020);
            result.Vin.ShouldBe("VIN00000000000001");
            result.Visits.ShouldBe(0);
            result.IsDeleted.ShouldBeFalse();
            result.CreatedOn.ShouldNotBe(default(DateTime));
            result.ModifiedOn.ShouldNotBe(default(DateTime));
        }
        finally
        {
            if (clientId.HasValue)
            {
                using var scope = ServiceProvider.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<ICarContext>();

                await ctx.Clients
                    .Where(x => x.Id == clientId.Value)
                    .ExecuteDeleteAsync(CancellationToken);
            }
        }
    }

    [Fact]
    public async Task Handle_Should_Return_Null_When_Car_Does_Not_Exist()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;
        var handler = sp.GetRequiredService<IRequestHandler<GetCarByIdCommand, CarDto?>>();
        var cmd = new GetCarByIdCommand(999999);

        var result = await handler.Handle(cmd, CancellationToken);

        result.ShouldBeNull();
    }

    [Fact]
    public async Task Handle_Should_Return_Car_With_Correct_Visit_Count()
    {
        int? clientId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<GetCarByIdCommand, CarDto?>>();

            var client = await new ClientBuilder(ctx)
                .WithDefaults(name: "Anna", surname: "Nowak", phoneNumber: "987654321")
                .WithCar("Honda", "Civic", 2021, "VIN20000000000002")
                .Build();

            clientId = client.Id;
            var car = client.Cars.First();
            
            car.IncrementVisits();
            car.IncrementVisits();
            car.IncrementVisits();
            await ctx.SaveChangesAsync(CancellationToken);

            var cmd = new GetCarByIdCommand(car.Id);

            var result = await handler.Handle(cmd, CancellationToken);

            result.ShouldNotBeNull();
            result.Visits.ShouldBe(3);
            result.Make.ShouldBe("Honda");
            result.Model.ShouldBe("Civic");
        }
        finally
        {
            if (clientId.HasValue)
            {
                using var scope = ServiceProvider.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<ICarContext>();

                await ctx.Clients
                    .Where(x => x.Id == clientId.Value)
                    .ExecuteDeleteAsync(CancellationToken);
            }
        }
    }

    [Fact]
    public async Task Handle_Should_Return_Deleted_Car_Information()
    {
        int? clientId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<GetCarByIdCommand, CarDto?>>();

            var client = await new ClientBuilder(ctx)
                .WithDefaults(name: "Jan", surname: "Kowalski", phoneNumber: "123456789")
                .WithCar("Toyota", "Corolla", 2020, "VIN00000000000001")
                .Build();

            clientId = client.Id;
            var car = client.Cars.First();

            car.Delete(car.Id);
            await ctx.SaveChangesAsync(CancellationToken);

            var cmd = new GetCarByIdCommand(car.Id);

            var result = await handler.Handle(cmd, CancellationToken);

            result.ShouldNotBeNull();
            result.IsDeleted.ShouldBeTrue();
            result.Make.ShouldBe("Toyota");
            result.Model.ShouldBe("Corolla");
        }
        finally
        {
            if (clientId.HasValue)
            {
                using var scope = ServiceProvider.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<ICarContext>();

                await ctx.Clients
                    .Where(x => x.Id == clientId.Value)
                    .ExecuteDeleteAsync(CancellationToken);
            }
        }
    }

    [Fact]
    public async Task Handle_Should_Return_Car_From_Specific_Client()
    {
        int? clientId1 = null;
        int? clientId2 = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<GetCarByIdCommand, CarDto?>>();

            var client1 = await new ClientBuilder(ctx)
                .WithDefaults(name: "Jan", surname: "Kowalski", phoneNumber: "123456789")
                .WithCar("Toyota", "Corolla", 2020, "VIN00000000000001")
                .Build();

            var client2 = await new ClientBuilder(ctx)
                .WithDefaults(name: "Janek", surname: "Kowalska", phoneNumber: "123456788")
                .WithCar("Audi", "A4", 2020, "VIN00000000000002")
                .Build();

            clientId1 = client1.Id;
            clientId2 = client2.Id;

            var client1CarId = client1.Cars.First().Id;
            var cmd = new GetCarByIdCommand(client1CarId);

            var result = await handler.Handle(cmd, CancellationToken);

            result.ShouldNotBeNull();
            result.CarId.ShouldBe(client1CarId);
            result.Make.ShouldBe("Toyota");
            result.Model.ShouldBe("Corolla");
        }
        finally
        {
            if (clientId1.HasValue || clientId2.HasValue)
            {
                using var scope = ServiceProvider.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<ICarContext>();

                if (clientId1.HasValue)
                {
                    await ctx.Clients.Where(x => x.Id == clientId1.Value).ExecuteDeleteAsync(CancellationToken);
                }

                if (clientId2.HasValue)
                {
                    await ctx.Clients.Where(x => x.Id == clientId2.Value).ExecuteDeleteAsync(CancellationToken);
                }
            }
        }
    }
}
