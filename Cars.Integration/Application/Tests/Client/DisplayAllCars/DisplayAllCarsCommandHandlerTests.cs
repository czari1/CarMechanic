using Cars.Application.Clients.DisplayAllCars;
using Cars.Application.Clients.Models;
using Cars.Application.Common;
using Cars.Integration.Application.Builder;
using Cars.Integration.Application.Tests.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Cars.Integration.Application.Tests.Client.DisplayAllCars;
[Trait("Category", "IntegrationTests")]

public class DisplayAllCarsCommandHandlerTests : ApplicationTestsBase
{
    [Fact]
    public async Task Handle_Should_Display_All_Non_Deleted_Cars()
    {
        int? clientId1 = null;
        int? clientId2 = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<DisplayAllCarsCommand, IEnumerable<CarListDto>>>();

            var client1 = await new ClientBuilder(ctx)
                .WithDefaults(name: "Jan", surname: "Kowalski", phoneNumber: "123456789")
                .WithCar("Toyota", "Corolla", 2020, "VIN00000000000001")
                .WithCar("Honda", "Civic", 2021, "VIN00000000000002")
                .Build();

            var client2 = await new ClientBuilder(ctx)
               .WithDefaults(name: "Janek", surname: "Kowalska", phoneNumber: "113456789")
               .WithCar("Ford", "Focus", 2019, "VIN00000000000003")
               .Build();


            clientId1 = client1.Id;
            clientId2 = client2.Id;

            var cmd = new DisplayAllCarsCommand();

            var result = await handler.Handle(cmd, CancellationToken);

            var carsList = result.ToList();
            carsList.Count.ShouldBe(3);
            carsList.ShouldContain(c => c.Make == "Toyota" && c.Model == "Corolla");
            carsList.ShouldContain(c => c.Make == "Honda" && c.Model == "Civic");
            carsList.ShouldContain(c => c.Make == "Ford" && c.Model == "Focus");
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

    [Fact]
    public async Task Handle_Should_Not_Return_Deleted_Cars()
    {
        int? clientId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<DisplayAllCarsCommand, IEnumerable<CarListDto>>>();

            var client = await new ClientBuilder(ctx)
                .WithDefaults(name: "Jan", surname: "Kowalski", phoneNumber: "123456789")
                .WithCar("Toyota", "Corolla", 2020, "VIN00000000000004")
                .Build();

            clientId = client.Id;

            var car = client.Cars.First();
            car.Delete(car.Id);
            await ctx.SaveChangesAsync(CancellationToken);

            var cmd = new DisplayAllCarsCommand();

            var result = await handler.Handle(cmd, CancellationToken);

            result.ShouldNotContain(c => c.Vin == "VIN00000000000004");

        }
        finally
        {
            if (clientId.HasValue)
            {
                using var scope = ServiceProvider.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<ICarContext>();
                await ctx.Clients.Where(x => x.Id == clientId.Value).ExecuteDeleteAsync(CancellationToken);
            }
        }
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_List_When_No_Cars_Exist()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;
        var ctx = sp.GetRequiredService<ICarContext>();
        var handler = sp.GetRequiredService<IRequestHandler<DisplayAllCarsCommand, IEnumerable<CarListDto>>>();
        var cmd = new DisplayAllCarsCommand();

        var result = await handler.Handle(cmd, CancellationToken);

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task Handle_Should_Return_Car_After_ReverseDelet()
    {
        int? clientId = null;
        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<DisplayAllCarsCommand, IEnumerable<CarListDto>>>();
            var cmd = new DisplayAllCarsCommand();

            var client = await new ClientBuilder(ctx)
                .WithDefaults(name: "Jan", surname: "Kowalski", phoneNumber: "123456789")
                .WithCar("Mazda", "MX-5", 2022, "VIN00000000000005")
                .Build();

            clientId = client.Id;
            var car = client.Cars.First();
            var carVin = car.VIN;

            car.Delete(car.Id);
            await ctx.SaveChangesAsync(CancellationToken);

            car.ReverseDelete(car.Id);
            await ctx.SaveChangesAsync(CancellationToken);

            var resultAfterRestore = await handler.Handle(cmd, CancellationToken);
            resultAfterRestore.ShouldContain(c => c.Vin == carVin && c.Make == "Mazda" && c.Model == "MX-5");
        }
        finally
        {
            if (clientId.HasValue)
            {
                using var scope = ServiceProvider.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<ICarContext>();
                await ctx.Clients.Where(x => x.Id == clientId.Value).ExecuteDeleteAsync(CancellationToken);
            }
        }
    }

    [Fact]
    public async Task Handle_Should_Not_Return_Deleted_Car_But_Return_Other_Cars_From_Same_Client()
    {
        int? clientId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<DisplayAllCarsCommand, IEnumerable<CarListDto>>>();
            var cmd = new DisplayAllCarsCommand();

            var client = await new ClientBuilder(ctx)
                .WithDefaults(name: "Anna", surname: "Nowak", phoneNumber: "987654321")
                .WithCar("BMW", "X5", 2021, "VIN00000000000006")
                .WithCar("BMW", "320i", 2020, "VIN00000000000007")
                .WithCar("BMW", "M3", 2022, "VIN00000000000008")
                .Build();

            clientId = client.Id;

            var carToDelete = client.Cars.FirstOrDefault(c => c.Model == "320i");
            carToDelete!.Delete(carToDelete.Id);
            await ctx.SaveChangesAsync(CancellationToken);

            var result = await handler.Handle(cmd, CancellationToken);

            var clientCars = result.Where(c => c.OwnerId == clientId.Value).ToList();
            clientCars.Count.ShouldBe(2);
            clientCars.ShouldContain(c => c.Model == "X5");
            clientCars.ShouldContain(c => c.Model == "M3");
            clientCars.ShouldNotContain(c => c.Model == "320i");
        }
        finally
        {
            if (clientId.HasValue)
            {
                using var scope = ServiceProvider.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<ICarContext>();
                await ctx.Clients.Where(x => x.Id == clientId.Value).ExecuteDeleteAsync(CancellationToken);
            }
        }
    }

}
