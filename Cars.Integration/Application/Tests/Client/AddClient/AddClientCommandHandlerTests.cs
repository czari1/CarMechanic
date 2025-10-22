using Cars.Application.Clients.AddClient;
using Cars.Application.Common;
using Cars.Integration.Application.Tests.Base;
using MediatR;
using Shouldly;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cars.Integration.Application.Tests.Client.AddClient;
[Trait("Category", "IntegrationTests")]
public class AddClientCommandHandlerTests : ApplicationTestsBase
{
    [Fact]
    public async Task Handle_Should_Add_Client_And_Return_ClientId()
    {
        int? clientId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;

            var ctx = sp.GetRequiredService<ICarContext>();

            var cmd = new AddClientCommand(
                            Id: 0,
                            Name: "Jan",
                            Surname: "Kowalski",
                            PhoneNumber: "123456789");

            var handler = sp.GetRequiredService<IRequestHandler<AddClientCommand, int>>();

            // ACT
            clientId = await handler.Handle(cmd, CancellationToken);

            // ASSERT
            var created = await ctx.Clients
                            .Where(x => x.Id == clientId)
                            .SingleOrDefaultAsync(CancellationToken);

            created.ShouldNotBeNull();
            created!.Name.ShouldBe("Jan");
            created.Surname.ShouldBe("Kowalski");
            created.PhoneNumber.ShouldBe("123456789");
            created.Cars.ShouldBeEmpty();
        }
        finally
        {
            if (clientId.HasValue)
            {
                var ct = CancellationToken.None;
                using var scope = ServiceProvider.CreateScope();
                var sp = scope.ServiceProvider;
                var ctx = sp.GetRequiredService<ICarContext>();

                await ctx.Clients
                         .Where(x => x.Id == clientId.Value)
                         .ExecuteDeleteAsync(ct);
            }
        }
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_Name_Is_Empty()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        var cmd = new AddClientCommand(
            Id: 0,
            Name: "",
            Surname: "Kowalski",
            PhoneNumber: "123456789");

        var handler = sp.GetRequiredService<IRequestHandler<AddClientCommand, int>>();

        await Should.ThrowAsync<FluentValidation.ValidationException>(
            async () => await handler.Handle(cmd, CancellationToken));
    }
    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_Surname_Is_Empty()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        var cmd = new AddClientCommand(
            Id: 0,
            Name: "Jan",
            Surname: "",
            PhoneNumber: "123456789");

        var handler = sp.GetRequiredService<IRequestHandler<AddClientCommand, int>>();

        await Should.ThrowAsync<FluentValidation.ValidationException>(
            async () => await handler.Handle(cmd, CancellationToken));
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_PhoneNumber_Is_Invalid()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        var cmd = new AddClientCommand(
            Id: 0,
            Name: "Jan",
            Surname: "Kowalski",
            PhoneNumber: "12789");

        var handler = sp.GetRequiredService<IRequestHandler<AddClientCommand, int>>();

        await Should.ThrowAsync<FluentValidation.ValidationException>(
            async () => await handler.Handle(cmd, CancellationToken));
    }
}
