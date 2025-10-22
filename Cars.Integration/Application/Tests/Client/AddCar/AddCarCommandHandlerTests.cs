using Cars.Application.Clients.AddCar;
using Cars.Application.Common;
using Cars.Integration.Application.Tests.Base;
using MediatR;
using Shouldly;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cars.Integration.Application.Tests.Client.AddClient;
[Trait("Category", "IntegrationTests")]
public class AddCarCommandHandlerTests : ApplicationTestsBase
{
    [Fact]
    public async Task Handle_Should_Add_Car_And_Return_CarId()
    {
        int? carId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;

            var ctx = sp.GetRequiredService<ICarContext>();

            var cmd = new AddCarCommand(
                            ClientId: 0,
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
            created!.Make.ShouldBe("Audi");
            created.Model.ShouldBe("A6");
            created.Year.ShouldBe(20);
            created.VIN.ShouldBe("WAUZZZ4G1GN080637");
        }
        finally
        {
            if (carId.HasValue)
            {
                var ct = CancellationToken.None;
                using var scope = ServiceProvider.CreateScope();
                var sp = scope.ServiceProvider;
                var ctx = sp.GetRequiredService<IQueryContext>();

                await ctx.CarQuery
                         .Where(x => x.Id == carId.Value)
                         .ExecuteDeleteAsync(ct);
            }
        }
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_Name_Is_Empty()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        var cmd = new AddCarCommand(
                            ClientId: 0,
                            Make: "Audi",
                            Model: "A6",
                            Year: 2000,
                            Vin: "WAUZZZ4G1GN080637");

        var handler = sp.GetRequiredService<IRequestHandler<AddCarCommand, int>>();

        await Should.ThrowAsync<FluentValidation.ValidationException>(
            async () => await handler.Handle(cmd, CancellationToken));
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_Surname_Is_Empty()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        var cmd = new AddCarCommand(
                            ClientId: 0,
                            Make: "Audi",
                            Model: "A6",
                            Year: 2000,
                            Vin: "WAUZZZ4G1GN080637");

        var handler = sp.GetRequiredService<IRequestHandler<AddCarCommand, int>>();

        await Should.ThrowAsync<FluentValidation.ValidationException>(
            async () => await handler.Handle(cmd, CancellationToken));
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_PhoneNumber_Is_Invalid()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        var cmd = new AddCarCommand(
                            ClientId: 0,
                            Make: "Audi",
                            Model: "A6",
                            Year: 2000,
                            Vin: "WAUZZZ4G1GN080637");

        var handler = sp.GetRequiredService<IRequestHandler<AddCarCommand, int>>();

        await Should.ThrowAsync<FluentValidation.ValidationException>(
            async () => await handler.Handle(cmd, CancellationToken));
    }
}
