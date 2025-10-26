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
            var entity = await new ClientBuilder(ctx, clientId)
                                    .WithDefaults()
                                    .WithCars(2)
                                    .Build();

            ctx.Add(entity);    
            await ctx.SaveChangesAsync(CancellationToken);
            clientId = entity.Id;

            var cmd = new DeleteClientCommand(entity.Id);

            // ACT
            await handler.Handle(cmd, CancellationToken);

            // ASSERT
            using var verifyScope = ServiceProvider.CreateScope();
            var vctx = verifyScope.ServiceProvider.GetRequiredService<ICarContext>();

            var reloaded = await vctx.Clients.SingleOrDefaultAsync(x => x.Id == entity.Id, CancellationToken);

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
}