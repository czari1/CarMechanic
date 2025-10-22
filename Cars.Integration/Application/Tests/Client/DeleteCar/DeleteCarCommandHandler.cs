using Cars.Application.Clients.DeleteClient;
using Cars.Application.Common;
using Cars.Integration.Application.Builder;
using Cars.Integration.Application.Tests.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cars.Integration.Application.Tests.Client.DeleteClient;
[Trait("Category", "IntegrationTests")]
public class DeleteCarCommandHandlerTests : ApplicationTestsBase
{
    [Fact]
    public async Task Handle_Should_SoftDelete_When_Status_Is_Not_Draft()
    {
        int? surveyId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<DeleteClientCommand>>();

            // ARRANGE:
            var entity = new CarBuilder()
                                .WithDefaults(name: "Delete published test", baseLang: "en")
                                .Build();

            ctx.Add(entity);
            await ctx.SaveChangesAsync(CancellationToken);
            surveyId = entity.Id;

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
            if (surveyId.HasValue)
            {
                using var scope = ServiceProvider.CreateScope();
                var sp = scope.ServiceProvider;
                var ctx = sp.GetRequiredService<ICarContext>();

                await ctx.Clients
                            .Where(x => x.Id == surveyId.Value)
                            .ExecuteDeleteAsync(CancellationToken);
            }
        }
    }
}