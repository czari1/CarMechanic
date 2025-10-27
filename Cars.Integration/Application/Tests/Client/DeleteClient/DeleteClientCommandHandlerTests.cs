using Cars.Application.Clients.DeleteClient;
using Cars.Application.Common;
using Cars.Integration.Application.Builder;
using Cars.Integration.Application.Tests.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Cars.Integration.Application.Tests.Client.DeleteClient;
[Trait("Category", "IntegrationTests")]
public class DeleteClientCommandHandlerTests : ApplicationTestsBase
{
    [Fact]
    public async Task Handle_Should_Also_Mark_Cars_As_Deleted()
    {
        int? clientId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<DeleteClientCommand>>();

            // ARRANGE:
            var client = await new ClientBuilder(ctx)
                                    .WithDefaults()
                                    .WithCars(2)
                                    .Build();
            clientId = client.Id;

            var cmd = new DeleteClientCommand(client.Id);

            await handler.Handle(cmd, CancellationToken);

            using var verifyScope = ServiceProvider.CreateScope();
            var vctx = verifyScope.ServiceProvider.GetRequiredService<ICarContext>();

            var reloaded = await vctx.Clients
                .SingleOrDefaultAsync(x => x.Id == client.Id, CancellationToken);

            reloaded.ShouldBeNull();

        }
        finally
        {
            if (clientId.HasValue)
            {
                using var scope = ServiceProvider.CreateScope();
                var sp = scope.ServiceProvider;
                var ctx = sp.GetRequiredService<ICarContext>();

                await ctx.Clients
                    .Where(x => x.Id == clientId.Value)
                    .ExecuteDeleteAsync(CancellationToken);
            }
        }
    }

    [Fact]
    public async Task Handle_Should_Also_Delete_Associated_Cars()
    {
        int? clientId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<DeleteClientCommand>>();

            var client = await new ClientBuilder(ctx)
                .WithDefaults()
                .WithCar("Toyota", "Corolla", 2020, "VIN10000000000001")
                .WithCar("Honda", "Civic", 2021, "VIN10000000000002")
                .Build();

            clientId = client.Id;
            var carIds = client.Cars.Select(c => c.Id).ToList();

            var cmd = new DeleteClientCommand(client.Id);

            await handler.Handle(cmd, CancellationToken);

            using var verifyScope = ServiceProvider.CreateScope();
            var vctx = verifyScope.ServiceProvider.GetRequiredService<ICarContext>();

            var reloadedClient = await vctx.Clients
                .SingleOrDefaultAsync(x => x.Id == client.Id, CancellationToken);
            reloadedClient.ShouldBeNull();

            foreach (var carId in carIds)
            {
                var car = await vctx.CarQuery
                    .SingleOrDefaultAsync(c => c.Id == carId, CancellationToken);
                car.ShouldBeNull();
            }
        }
        finally
        {
            if (clientId.HasValue)
            {
                using var scope = ServiceProvider.CreateScope();
                var sp = scope.ServiceProvider;
                var ctx = sp.GetRequiredService<ICarContext>();

                await ctx.Clients
                    .Where(x => x.Id == clientId.Value)
                    .ExecuteDeleteAsync(CancellationToken);
            }
        }
    }

    [Fact]
    public async Task Handle_Should_Throw_KeyNotFoundException_When_Client_Does_Not_Exist()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;
        var handler = sp.GetRequiredService<IRequestHandler<DeleteClientCommand>>();

        // ARRANGE
        var cmd = new DeleteClientCommand(999999);

        // ACT & ASSERT
        await Should.ThrowAsync<KeyNotFoundException>(
            async () => await handler.Handle(cmd, CancellationToken));
    }
}