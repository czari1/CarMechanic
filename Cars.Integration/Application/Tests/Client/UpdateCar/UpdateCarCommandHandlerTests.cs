using Cars.Application.Clients.UpdateCar;
using Cars.Application.Common;
using Cars.Domain.Entities;
using Cars.Integration.Application.Builder;
using Cars.Integration.Application.Tests.Base;
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
            
            ctx.Add(client);
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
}