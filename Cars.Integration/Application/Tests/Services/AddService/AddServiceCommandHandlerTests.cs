using Cars.Application.Services.AddService;
using Cars.Application.Common;
using Cars.Integration.Application.Tests.Base;
using MediatR;
using Shouldly;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cars.Integration.Application.Tests.Client.AddClient;
[Trait("Category", "IntegrationTests")]
public class AddServiceCommandHandlerTests : ApplicationTestsBase
{
    [Fact]
    public async Task Handle_Should_Add_Service_And_Return_ServiceId()
    {
        int? serviceId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;

            var ctx = sp.GetRequiredService<ICarContext>();

            var cmd = new AddServiceCommand(
                            ServiceId: 0,
                            ServiceName: "Wymiana",
                            ServiceDescription: "Wymiana skrzyni",
                            Price: 200.0m,
                            ServiceDate: DateTime.UtcNow);

            var handler = sp.GetRequiredService<IRequestHandler<AddServiceCommand, int>>();

            // ACT
            serviceId = await handler.Handle(cmd, CancellationToken);

            // ASSERT
            var created = await ctx.Services
                            .Where(x => x.Id == serviceId)
                            .SingleOrDefaultAsync(CancellationToken);

            created.ShouldNotBeNull();
            created.ServiceName.ShouldBe("Wymiana");
            created.ServiceDescription.ShouldBe("Wymiana skrzyni");
            created.Price.ShouldBe(200.0m);
            created.ServiceDate.ShouldBeOfType<DateTime>();
        }
        finally
        {
            if (serviceId.HasValue)
            {
                var ct = CancellationToken.None;
                using var scope = ServiceProvider.CreateScope();
                var sp = scope.ServiceProvider;
                var ctx = sp.GetRequiredService<ICarContext>();

                await ctx.Services
                         .Where(x => x.Id == serviceId.Value)
                         .ExecuteDeleteAsync(ct);
            }
        }
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_ServiceName_Is_Empty()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        var cmd = new AddServiceCommand(
            ServiceId: 0,
            ServiceName: "",
            ServiceDescription: "Wymiana skrzyni",
            Price: 200.0m,
            ServiceDate: DateTime.UtcNow);

        var handler = sp.GetRequiredService<IRequestHandler<AddServiceCommand, int>>();

        await Should.ThrowAsync<FluentValidation.ValidationException>(
            async () => await handler.Handle(cmd, CancellationToken));
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_ServiceDescription_Is_Empty()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        var cmd = new AddServiceCommand(
            ServiceId: 0,
            ServiceName: "",
            ServiceDescription: "Wymiana skrzyni",
            Price: 200.0m,
            ServiceDate: DateTime.UtcNow);

        var handler = sp.GetRequiredService<IRequestHandler<AddServiceCommand, int>>();

        await Should.ThrowAsync<FluentValidation.ValidationException>(
            async () => await handler.Handle(cmd, CancellationToken));
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_Price_Is_Negative()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        var cmd = new AddServiceCommand(
            ServiceId: 0,
            ServiceName: "Wymiana",
            ServiceDescription: "Wymiana skrzyni",
            Price: -1.0m,
            ServiceDate: DateTime.UtcNow);

        var handler = sp.GetRequiredService<IRequestHandler<AddServiceCommand, int>>();

        await Should.ThrowAsync<FluentValidation.ValidationException>(
            async () => await handler.Handle(cmd, CancellationToken));
    }
}
