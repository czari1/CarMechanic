using Cars.Application.Clients.AddCar;
using Cars.Application.Common;
using Cars.Integration.Application.Builder;
using Cars.Integration.Application.Tests.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Cars.Integration.Application.Tests.Client.AddCar;
[Trait("Category", "IntegrationTests")]
public class AddCarCommandHandlerTests : ApplicationTestsBase
{
    [Fact]
    public async Task Handle_Should_Add_Car_And_Return_CarId()
    {
        int? carId = null;
        int? clientId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;

            var ctx = sp.GetRequiredService<ICarContext>();

            var client = await new ClientBuilder(ctx)
                .WithDefaults(name: "Jan", surname: "Kowalski")
                .Build();

            clientId = client.Id;

            var cmd = new AddCarCommand(
                            ClientId: clientId.Value,
                            Make: "Audi",
                            Model: "A6",
                            Year: 2000,
                            Vin: "WAUZZZ4G1GN080637");

            var handler = sp.GetRequiredService<IRequestHandler<AddCarCommand, int>>();

            // ACT
            carId = await handler.Handle(cmd, CancellationToken);

            // ASSERT
            var created = await ctx.CarQuery
                            .Where(x => x.Id == carId)
                            .SingleOrDefaultAsync(CancellationToken);

            created.ShouldNotBeNull();
            created.Make.ShouldBe("Audi");
            created.Model.ShouldBe("A6");
            created.Year.ShouldBe(2000);
            created.VIN.ShouldBe("WAUZZZ4G1GN080637");
        }
        finally
        {
            var ct = CancellationToken.None;
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();

            if (carId.HasValue)
            {
                await ctx.CarQuery
                         .Where(x => x.Id == carId.Value)
                         .ExecuteDeleteAsync(ct);
            }

            if (clientId.HasValue)
            {
                await ctx.Clients
                         .Where(x => x.Id == clientId.Value)
                         .ExecuteDeleteAsync(ct);
            }
        }
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Client_Not_Found()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        var cmd = new AddCarCommand(
                            ClientId: 999, // Nieistniejący klient
                            Make: "Audi",
                            Model: "A6",
                            Year: 2000,
                            Vin: "WAUZZZ4G1GN080637");

        var handler = sp.GetRequiredService<IRequestHandler<AddCarCommand, int>>();

        // ACT & ASSERT
        await Should.ThrowAsync<KeyNotFoundException>( 
            async () => await handler.Handle(cmd, CancellationToken));
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_Make_Is_Empty()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        var cmd = new AddCarCommand(
                            ClientId: 0,
                            Make: "",
                            Model: "A6",
                            Year: 2000,
                            Vin: "WAUZZZ4G1GN080637");

        var handler = sp.GetRequiredService<IRequestHandler<AddCarCommand, int>>();

        await Should.ThrowAsync<FluentValidation.ValidationException>(
            async () => await handler.Handle(cmd, CancellationToken));
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_Model_Is_Empty()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        var cmd = new AddCarCommand(
                            ClientId: 0,
                            Make: "Audi",
                            Model: "",
                            Year: 2000,
                            Vin: "WAUZZZ4G1GN080637");

        var handler = sp.GetRequiredService<IRequestHandler<AddCarCommand, int>>();

        await Should.ThrowAsync<FluentValidation.ValidationException>(
            async () => await handler.Handle(cmd, CancellationToken));
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_VIN_Is_Invalid()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        var cmd = new AddCarCommand(
                            ClientId: 0,
                            Make: "Audi",
                            Model: "A6",
                            Year: 2000,
                            Vin: "");

        var handler = sp.GetRequiredService<IRequestHandler<AddCarCommand, int>>();

        await Should.ThrowAsync<FluentValidation.ValidationException>(
            async () => await handler.Handle(cmd, CancellationToken));
    }
    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_Year_Is_Invalid()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        var cmd = new AddCarCommand(
                            ClientId: 0,
                            Make: "Audi",
                            Model: "A6",
                            Year: 20,
                            Vin: "WAUZZZ4G1GN080637");

        var handler = sp.GetRequiredService<IRequestHandler<AddCarCommand, int>>();

        await Should.ThrowAsync<FluentValidation.ValidationException>(
            async () => await handler.Handle(cmd, CancellationToken));
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_VIN_Is_Not_Unique()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        var cmd = new AddCarCommand(
                            ClientId: 0,
                            Make: "Audi",
                            Model: "A6",
                            Year: 20,
                            Vin: "WAUZZZ4G1GN080637");
        
        var cmd1 = new AddCarCommand(
                            ClientId: 0,
                            Make: "Car",
                            Model: "A6",
                            Year: 2000,
                            Vin: "WAUZZZ4G1GN080637");

        var handler = sp.GetRequiredService<IRequestHandler<AddCarCommand, int>>();

        await Should.ThrowAsync<FluentValidation.ValidationException>(
            async () => await handler.Handle(cmd, CancellationToken));
    }
}
