using Cars.Application.Common;
using Cars.Application.Services.DeleteService;
using Cars.Integration.Application.Builder;
using Cars.Integration.Application.Tests.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Cars.Integration.Application.Tests.Services.DeleteService;

[Trait("Category", "IntegrationTests")]
public class DeleteServiceCommandHandlerTests : ApplicationTestsBase
{
    [Fact]
    public async Task Handle_Should_Delete_Service_Successfully()
    {
        int? serviceId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<DeleteServiceCommand>>();

            var service = await new ServiceBuilder(ctx)
                .WithDefaults()
                .Build();

            serviceId = service.Id;

            var cmd = new DeleteServiceCommand(serviceId.Value);

            await handler.Handle(cmd, CancellationToken);

            using var verifyScope = ServiceProvider.CreateScope();
            var vctx = verifyScope.ServiceProvider.GetRequiredService<ICarContext>();

            var deleted = await vctx.Services
                .SingleOrDefaultAsync(x => x.Id == serviceId.Value, CancellationToken);

            deleted.ShouldBeNull();
        }
        finally
        {
            if (serviceId.HasValue)
            {
                using var scope = ServiceProvider.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<ICarContext>();

                await ctx.Services
                    .Where(x => x.Id == serviceId.Value)
                    .ExecuteDeleteAsync(CancellationToken);
            }
        }
    }

    [Fact]
    public async Task Handle_Should_Throw_KeyNotFoundException_When_Service_Does_Not_Exist()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;
        var handler = sp.GetRequiredService<IRequestHandler<DeleteServiceCommand>>();

        var cmd = new DeleteServiceCommand(999999);

        await Should.ThrowAsync<KeyNotFoundException>(
            async () => await handler.Handle(cmd, CancellationToken));
    }
}