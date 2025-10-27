using Cars.Application.Clients.UpdateCar;
using Cars.Application.Clients.UpdateClient;
using Cars.Application.Common;
using Cars.Integration.Application.Builder;
using Cars.Integration.Application.Tests.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Cars.Integration.Application.Tests.Client.UpdateClient;
[Trait("Category", "IntegrationTests")]
public class UpdateClientCommandHandlerTests : ApplicationTestsBase
{
    [Fact]
    public async Task Handle_Should_Update_Client_Successfully()
    {
        int? clientId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<UpdateClientCommand>>();

            var client = await new ClientBuilder(ctx)
                .WithDefaults()
                .WithCar("Toyota", "Corolla", 2020, "12345678901234567")
                .Build();

            await ctx.SaveChangesAsync(CancellationToken);

            clientId = client.Id;

            var cmd = new UpdateClientCommand(clientId.Value, "Tomek", "Kowal", "123123123");

            await handler.Handle(cmd, CancellationToken);

            using var verifyScope = ServiceProvider.CreateScope();
            var vctx = verifyScope.ServiceProvider.GetRequiredService<ICarContext>();

            var updatedClient = await vctx.Clients
                .FirstOrDefaultAsync(c => c.Id == clientId.Value, CancellationToken);

            updatedClient.Name.ShouldBe("Tomek");
            updatedClient.Surname.ShouldBe("Kowal");
            updatedClient.PhoneNumber.ShouldBe("123123123");
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
