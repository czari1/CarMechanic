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
                .Build();

            client1.AddCar("Toyota", "Corolla", 2020, "1HGBH41JXMN109186");
            client1.AddCar("Honda", "Civic", 2021, "2HGBH41JXMN109187");

            var client2 = await new ClientBuilder(ctx)
                .WithDefaults(name: "Janek", surname: "Kowalska", phoneNumber: "113456789")
                .Build();

            client2.AddCar("Ford", "Focus", 2019, "3HGBH41JXMN109188");

            ctx.Add(client1);
            ctx.Add(client2);
            await ctx.SaveChangesAsync(CancellationToken);

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
                .Build();

            client.AddCar("Toyota", "Corolla", 2020, "1HGBH41JXMN109186");

            ctx.Add(client);
            await ctx.SaveChangesAsync(CancellationToken);
            clientId = client.Id;

            var car = client.Cars.First();
            car.Delete(car.Id);
            await ctx.SaveChangesAsync(CancellationToken);

            var cmd = new DisplayAllCarsCommand();

            var result = await handler.Handle(cmd, CancellationToken);

            result.ShouldNotContain(c => c.Vin == "1HGBH41JXMN109186");

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

        result.ShouldBeNull();
        result.ShouldBeEmpty();
    }
}
