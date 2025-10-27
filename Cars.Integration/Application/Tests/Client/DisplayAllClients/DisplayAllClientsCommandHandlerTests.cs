using Cars.Application.Clients.DisplayAllClients;
using Cars.Application.Clients.Models;
using Cars.Application.Common;
using Cars.Integration.Application.Builder;
using Cars.Integration.Application.Tests.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Cars.Integration.Application.Tests.Client.DisplayAllClients;
[Trait("Category", "IntegrationTests")]

public class DisplayAllClientsCommandHandlerTests : ApplicationTestsBase
{
    [Fact]
    public async Task Handle_Should_Display_All_Non_Deleted_Clients()
    {
        int? clientId1 = null;
        int? clientId2 = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<DisplayAllClientsCommand, IEnumerable<ClientListDto>>>();

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

            var cmd = new DisplayAllClientsCommand();

            var result = await handler.Handle(cmd, CancellationToken);

            var clientsList = result.ToList();
            clientsList.Count.ShouldBe(2);
            clientsList.ShouldContain(c => c.Name == "Jan" && c.Surname == "Kowalski");
            clientsList.ShouldContain(c => c.Name == "Janek" && c.Surname == "Kowalska");
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
    public async Task Handle_Should_Not_Return_Deleted_Clients()
    {
        int? clientId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<DisplayAllClientsCommand, IEnumerable<ClientListDto>>>();

            var client = await new ClientBuilder(ctx)
                .WithDefaults(name: "Jan", surname: "Kowalski", phoneNumber: "123456789")
                .WithCar("Toyota", "Corolla", 2020, "VIN00000000000004")
                .Build();

            clientId = client.Id;

            ctx.Delete(client);

            await ctx.SaveChangesAsync(CancellationToken);

            var cmd = new DisplayAllClientsCommand();

            var result = await handler.Handle(cmd, CancellationToken);

            result.ShouldNotContain(c => c.Name == "Jan" && c.Surname == "Kowalski");

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
    public async Task Handle_Should_Return_Empty_List_When_No_Clients_Exist()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;
        var ctx = sp.GetRequiredService<ICarContext>();
        var handler = sp.GetRequiredService<IRequestHandler<DisplayAllClientsCommand, IEnumerable<ClientListDto>>>();
        var cmd = new DisplayAllClientsCommand();

        var result = await handler.Handle(cmd, CancellationToken);

        result.ShouldBeEmpty();
    }
}
