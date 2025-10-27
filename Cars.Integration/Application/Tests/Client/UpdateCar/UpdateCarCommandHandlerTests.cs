using Cars.Application.Clients.UpdateCar;
using Cars.Application.Common;
using Cars.Domain.Entities;
using Cars.Integration.Application.Builder;
using Cars.Integration.Application.Tests.Base;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Cars.Integration.Application.Tests.Client.UpdateCar;

[Trait("Category", "IntegrationTests")]
public class UpdateCarCommandHandlerTests : ApplicationTestsBase
{
    [Fact]
    public async Task Handle_Should_Update_Car_Successfully()
    {
        int? clientId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<UpdateCarCommand>>();

            var client = await new ClientBuilder(ctx)
                .WithDefaults()
                .WithCar("Toyota", "Corolla", 2020, "12345678901234567")
                .Build();
            
            await ctx.SaveChangesAsync(CancellationToken);
            
            clientId = client.Id;
            var carId = client.Cars.First().Id;

            var cmd = new UpdateCarCommand(clientId.Value, carId, "", "Avensis", 2010);

            await handler.Handle(cmd, CancellationToken);

            using var verifyScope = ServiceProvider.CreateScope();
            var vctx = verifyScope.ServiceProvider.GetRequiredService<ICarContext>();

            var updatedClient = await vctx.Clients
                .Include(c => c.Cars)
                .FirstOrDefaultAsync(c => c.Id == clientId.Value, CancellationToken);

            var updatedCar = updatedClient!.Cars.First(c => c.Id == carId);
            updatedCar.Make.ShouldBe("Toyota"); 
            updatedCar.Model.ShouldBe("Avensis");
            updatedCar.Year.ShouldBe(2010);
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
    public async Task Handle_Should_Update_Only_Make()
    {
        int? clientId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<UpdateCarCommand>>();

            var client = await new ClientBuilder(ctx)
                .WithDefaults()
                .WithCar("Toyota", "Corolla", 2020, "12345678901234567")
                .Build();

            clientId = client.Id;
            var carId = client.Cars.First().Id;

            var cmd = new UpdateCarCommand(clientId.Value, carId, "Honda", "", 2020);

            await handler.Handle(cmd, CancellationToken);

            using var verifyScope = ServiceProvider.CreateScope();
            var vctx = verifyScope.ServiceProvider.GetRequiredService<ICarContext>();

            var updatedClient = await vctx.Clients
                .Include(c => c.Cars)
                .FirstOrDefaultAsync(c => c.Id == clientId.Value, CancellationToken);

            var updatedCar = updatedClient!.Cars.First(c => c.Id == carId);
            updatedCar.Make.ShouldBe("Honda");
            updatedCar.Model.ShouldBe("Corolla");
            updatedCar.Year.ShouldBe(2020);
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
    public async Task Handle_Should_Update_Only_Model()
    {
        int? clientId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<UpdateCarCommand>>();

            var client = await new ClientBuilder(ctx)
                .WithDefaults()
                .WithCar("Toyota", "Corolla", 2020, "12345678901234567")
                .Build();

            clientId = client.Id;
            var carId = client.Cars.First().Id;

            var cmd = new UpdateCarCommand(clientId.Value, carId, "", "Avensis", 2020);

            await handler.Handle(cmd, CancellationToken);

            using var verifyScope = ServiceProvider.CreateScope();
            var vctx = verifyScope.ServiceProvider.GetRequiredService<ICarContext>();

            var updatedClient = await vctx.Clients
                .Include(c => c.Cars)
                .FirstOrDefaultAsync(c => c.Id == clientId.Value, CancellationToken);

            var updatedCar = updatedClient!.Cars.First(c => c.Id == carId);
            updatedCar.Make.ShouldBe("Toyota");
            updatedCar.Model.ShouldBe("Avensis");
            updatedCar.Year.ShouldBe(2020);
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
    public async Task Handle_Should_Update_Only_Year()
    {
        int? clientId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<UpdateCarCommand>>();

            var client = await new ClientBuilder(ctx)
                .WithDefaults()
                .WithCar("Toyota", "Corolla", 2020, "12345678901234567")
                .Build();

            clientId = client.Id;
            var carId = client.Cars.First().Id;

            var cmd = new UpdateCarCommand(clientId.Value, carId, "", "", 2021);

            await handler.Handle(cmd, CancellationToken);

            using var verifyScope = ServiceProvider.CreateScope();
            var vctx = verifyScope.ServiceProvider.GetRequiredService<ICarContext>();

            var updatedClient = await vctx.Clients
                .Include(c => c.Cars)
                .FirstOrDefaultAsync(c => c.Id == clientId.Value, CancellationToken);

            var updatedCar = updatedClient!.Cars.First(c => c.Id == carId);
            updatedCar.Make.ShouldBe("Toyota");
            updatedCar.Model.ShouldBe("Corolla");
            updatedCar.Year.ShouldBe(2021);
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
    public async Task Handle_Should_Not_Update_When_Client_Does_Not_Exist()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;
        var handler = sp.GetRequiredService<IRequestHandler<UpdateCarCommand>>();

        var cmd = new UpdateCarCommand(99999, 1, "Honda", "Civic", 2022);

        await handler.Handle(cmd, CancellationToken);
    }
    [Fact]
    public async Task Handle_Should_Throw_When_Make_Exceeds_Max_Length()
    {
        int? clientId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<UpdateCarCommand>>();

            var client = await new ClientBuilder(ctx)
               .WithDefaults()
               .WithCar("Toyota", "Corolla", 2020, "12345678901234567")
               .Build();

            clientId = client.Id;
            var carId = client.Cars.First().Id;

            var longMake = new string('A', 21);

            var cmd = new UpdateCarCommand(clientId.Value, carId, longMake, "Civic", 2020);

            await Should.ThrowAsync<ValidationException>(
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
}