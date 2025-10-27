using Cars.Application.Clients.DeleteCar;
using Cars.Application.Common;
using Cars.Integration.Application.Builder;
using Cars.Integration.Application.Tests.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Cars.Integration.Application.Tests.Client.DeleteCar;

[Trait("Category", "IntegrationTests")]
public class DeleteCarCommandHandlerTests : ApplicationTestsBase
{
    [Fact]
    public async Task Handle_Should_Delete_Car_Successfully()
    {
        int? clientId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<DeleteCarCommand>>();

            // ARRANGE
            var client = await new ClientBuilder(ctx)
                .WithDefaults()
                .WithCar("Toyota", "Corolla", 2020, "VIN60000000000001")
                .Build();

            clientId = client.Id;
            var carId = client.Cars.First().Id;

            var cmd = new DeleteCarCommand(clientId.Value, carId);

            // ACT
            await handler.Handle(cmd, CancellationToken);

            // ASSERT
            using var verifyScope = ServiceProvider.CreateScope();
            var vctx = verifyScope.ServiceProvider.GetRequiredService<ICarContext>();

            var updatedClient = await vctx.Clients
                .Include(c => c.Cars)
                .FirstOrDefaultAsync(c => c.Id == clientId.Value, CancellationToken);

            updatedClient.ShouldNotBeNull();
            updatedClient.Cars.ShouldBeEmpty();
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
    public async Task Handle_Should_Delete_Specific_Car_When_Client_Has_Multiple_Cars()
    {
        int? clientId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<DeleteCarCommand>>();

            // ARRANGE: Client with multiple cars
            var client = await new ClientBuilder(ctx)
                .WithDefaults()
                .WithCar("Toyota", "Corolla", 2020, "VIN60000000000002")
                .WithCar("Honda", "Civic", 2021, "VIN60000000000003")
                .WithCar("BMW", "X5", 2022, "VIN60000000000004")
                .Build();

            clientId = client.Id;
            var carToDelete = client.Cars.First(c => c.Model == "Civic");

            var cmd = new DeleteCarCommand(clientId.Value, carToDelete.Id);

            // ACT
            await handler.Handle(cmd, CancellationToken);

            // ASSERT
            using var verifyScope = ServiceProvider.CreateScope();
            var vctx = verifyScope.ServiceProvider.GetRequiredService<ICarContext>();

            var updatedClient = await vctx.Clients
                .Include(c => c.Cars)
                .FirstOrDefaultAsync(c => c.Id == clientId.Value, CancellationToken);

            updatedClient.ShouldNotBeNull();
            updatedClient.Cars.Count.ShouldBe(2);
            updatedClient.Cars.ShouldNotContain(c => c.Model == "Civic");
            updatedClient.Cars.ShouldContain(c => c.Model == "Corolla");
            updatedClient.Cars.ShouldContain(c => c.Model == "X5");
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
    public async Task Handle_Should_Throw_When_Client_Does_Not_Exist()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;
        var handler = sp.GetRequiredService<IRequestHandler<DeleteCarCommand>>();

        // ARRANGE: Non-existent client
        var cmd = new DeleteCarCommand(999999, 1);

        // ACT & ASSERT
        await Should.ThrowAsync<KeyNotFoundException>(
            async () => await handler.Handle(cmd, CancellationToken));
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Car_Does_Not_Exist()
    {
        int? clientId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<DeleteCarCommand>>();

            // ARRANGE: Client exists but car doesn't
            var client = await new ClientBuilder(ctx)
                .WithDefaults()
                .Build();

            clientId = client.Id;

            var cmd = new DeleteCarCommand(clientId.Value, 999999);

            // ACT & ASSERT
            await Should.ThrowAsync<ArgumentException>(
                async () => await handler.Handle(cmd, CancellationToken));
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
    public async Task Handle_Should_Not_Affect_Client_When_Deleting_Last_Car()
    {
        int? clientId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<DeleteCarCommand>>();

            // ARRANGE
            var client = await new ClientBuilder(ctx)
                .WithDefaults(name: "Jan", surname: "Kowalski")
                .WithCar("Toyota", "Corolla", 2020, "VIN60000000000005")
                .Build();

            clientId = client.Id;
            var carId = client.Cars.First().Id;

            var cmd = new DeleteCarCommand(clientId.Value, carId);

            // ACT
            await handler.Handle(cmd, CancellationToken);

            // ASSERT
            using var verifyScope = ServiceProvider.CreateScope();
            var vctx = verifyScope.ServiceProvider.GetRequiredService<ICarContext>();

            var updatedClient = await vctx.Clients
                .FirstOrDefaultAsync(c => c.Id == clientId.Value, CancellationToken);

            updatedClient.ShouldNotBeNull();
            updatedClient.Name.ShouldBe("Jan");
            updatedClient.Surname.ShouldBe("Kowalski");
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
}