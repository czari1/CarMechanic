using Cars.Application.Clients.GetCarById;
using Cars.Application.Clients.GetClientById;
using Cars.Application.Clients.Models;
using Cars.Application.Common;
using Cars.Integration.Application.Builder;
using Cars.Integration.Application.Tests.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Cars.Integration.Application.Tests.Client.GetClientById;

[Trait("Category", "IntegrationTests")]
public class GetClientByIdCommandHandlerTests : ApplicationTestsBase
{
    [Fact]
    public async Task Handle_Should_Get_Client_With_Basic_Information()
    {
        int? clientId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<GetClientByIdCommand, ClientDto?>>();

            var client = await new ClientBuilder(ctx)
                .WithDefaults(name: "Jan", surname: "Kowalski", phoneNumber: "123456789")
                .Build();

            clientId = client.Id;

            var cmd = new GetClientByIdCommand(client.Id);

            var result = await handler.Handle(cmd, CancellationToken);

            result.ShouldNotBeNull();
            result.Id.ShouldBe(clientId.Value);
            result.Name.ShouldBe("Jan");
            result.Surname.ShouldBe("Kowalski");
            result.PhoneNumber.ShouldBe("123456789");
            result.CreatedOn.ShouldNotBe(default(DateTime));
            result.ModifiedOn.ShouldNotBe(default(DateTime));
            result.Cars.ShouldBeEmpty();
        }
        finally
        {
            if (clientId.HasValue)
            {
                using var scope = ServiceProvider.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<ICarContext>();
                await ctx.Clients.Where(x => x.Id == clientId.Value).ExecuteDeleteAsync(CancellationToken);
            }
        }
    }

    [Fact]
    public async Task Handle_Should_Return_Null_When_Client_Does_Not_Exist()
    {
        using var scope = ServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;
        var handler = sp.GetRequiredService<IRequestHandler<GetClientByIdCommand, ClientDto?>>();

        var cmd = new GetClientByIdCommand(9999999);

        var result = await handler.Handle(cmd, CancellationToken);

        result.ShouldBeNull();
    }
    [Fact]
    public async Task Handle_Should_Get_Client_With_Single_Car()
    {
        int? clientId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<GetClientByIdCommand, ClientDto?>>();

            var client = await new ClientBuilder(ctx)
                .WithDefaults(name: "Jan", surname: "Kowalski", phoneNumber: "123456789")
                .WithCar("Audi", "A4", 2020, "VIN00000000000001")
                .Build();

            clientId = client.Id;
            var cmd = new GetClientByIdCommand(client.Id);

            var result = await handler.Handle(cmd, CancellationToken);

            result.ShouldNotBeNull();
            result.Cars.Count.ShouldBe(1);

            var car = result.Cars.First();
            car.Make.ShouldBe("Audi");
            car.Model.ShouldBe("A4");
            car.Year.ShouldBe(2020);
            car.Vin.ShouldBe("VIN00000000000001");
            car.Visits.ShouldBe(0);
            car.IsDeleted.ShouldBeFalse();
        }
        finally
        {
            if (clientId.HasValue)
            {
                using var scope = ServiceProvider.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<ICarContext>();
                await ctx.Clients.Where(x => x.Id == clientId.Value).ExecuteDeleteAsync(CancellationToken);
            }
        }
    }
    [Fact]
    public async Task Handle_Should_Get_Client_With_Multiple_Cars()
    {
        int? clientId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<GetClientByIdCommand, ClientDto?>>();

            var client = await new ClientBuilder(ctx)
               .WithDefaults(name: "Jan", surname: "Kowalski", phoneNumber: "123456789")
               .WithCar("Audi", "A4", 2020, "VIN00000000000001")
               .WithCar("Audi", "A3", 2021, "VIN00000000000002")
               .WithCar("Audi", "A5", 2022, "VIN00000000000003")
               .Build();

            clientId = client.Id;
            var cmd = new GetClientByIdCommand(client.Id);

            var result = await handler.Handle(cmd, CancellationToken);

            result.ShouldNotBeNull();
            result.Cars.Count().ShouldBe(3);
            result.Cars.ShouldContain(c => c.Make == "Audi" && c.Model == "A4");
            result.Cars.ShouldContain(c => c.Make == "Audi" && c.Model == "A3");
            result.Cars.ShouldContain(c => c.Make == "Audi" && c.Model == "A5");
        }
        finally
        {
            if (clientId.HasValue)
            {
                using var scope = ServiceProvider.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<ICarContext>();
                await ctx.Clients.Where(x => x.Id == clientId.Value).ExecuteDeleteAsync(CancellationToken);
            }
        }
    }
    [Fact]
    public async Task Handle_Should_Return_Empty_Cars_Collection_When_Client_Has_No_Cars()
    {
        int? clientId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<GetClientByIdCommand, ClientDto?>>();

            var client = await new ClientBuilder(ctx)
                .WithDefaults(name: "Jan", surname: "Kowalski", phoneNumber: "123456789")
                .Build();

            clientId = client.Id;
            var cmd = new GetClientByIdCommand(client.Id);

            var result = await handler.Handle(cmd, CancellationToken);

            result.ShouldNotBeNull();
            result.Cars.ShouldBeEmpty();
        }
        finally
        {
            if (clientId.HasValue)
            {
                using var scope = ServiceProvider.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<ICarContext>();
                await ctx.Clients.Where(x => x.Id == clientId.Value).ExecuteDeleteAsync(CancellationToken);
            }
        }
    }
    [Fact]
    public async Task Handle_Should_Include_Deleted_Cars_In_Response()
    {
        int? clientId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<GetClientByIdCommand, ClientDto?>>();

            var client = await new ClientBuilder(ctx)
               .WithDefaults(name: "Jan", surname: "Kowalski", phoneNumber: "123456789")
               .WithCar("Audi", "A4", 2020, "VIN00000000000001")
               .WithCar("Audi", "A3", 2021, "VIN00000000000002")
               .Build();

            clientId = client.Id;

            var carToDelete = client.Cars.First(c => c.Model == "A4");
            carToDelete.Delete(carToDelete.Id);
            await ctx.SaveChangesAsync(CancellationToken);

            var cmd = new GetClientByIdCommand(client.Id);

            var result = await handler.Handle(cmd, CancellationToken);

            result.ShouldNotBeNull();
            result.Cars.Count.ShouldBe(2);

            var deletedCar = result.Cars.FirstOrDefault(c => c.Model == "A4");
            deletedCar.ShouldNotBeNull();
            deletedCar.IsDeleted.ShouldBeTrue();

            var activeCar = result.Cars.FirstOrDefault(c => c.Model == "A3");
            activeCar.ShouldNotBeNull();
            activeCar.IsDeleted.ShouldBeFalse();
        }
        finally
        {
            if (clientId.HasValue)
            {
                using var scope = ServiceProvider.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<ICarContext>();
                await ctx.Clients.Where(x => x.Id == clientId.Value).ExecuteDeleteAsync(CancellationToken);
            }
        }
    }
    [Fact]
    public async Task Handle_Should_Return_Cars_With_Correct_Visit_Counts()
    {
        int? clientId = null;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ICarContext>();
            var handler = sp.GetRequiredService<IRequestHandler<GetClientByIdCommand, ClientDto?>>();

            var client = await new ClientBuilder(ctx)
               .WithDefaults(name: "Jan", surname: "Kowalski", phoneNumber: "123456789")
               .WithCar("Audi", "A4", 2020, "VIN00000000000001")
               .WithCar("Audi", "A3", 2021, "VIN00000000000002")
               .Build();

            clientId = client.Id;

            var firstCar = client.Cars.First(c => c.Model == "A4");
            var secondCar = client.Cars.First(c => c.Model == "A3");

            firstCar.IncrementVisits();
            firstCar.IncrementVisits();
            firstCar.IncrementVisits();

            secondCar.IncrementVisits();
            secondCar.IncrementVisits();

            await ctx.SaveChangesAsync(CancellationToken);

            var cmd = new GetClientByIdCommand(client.Id);

            var result = await handler.Handle(cmd, CancellationToken);

            result.ShouldNotBeNull();

            var A4 = result.Cars.First(c => c.Model == "A4");
            A4.Visits.ShouldBe(3);

            var A3 = result.Cars.First(c => c.Model == "A3");
            A3.Visits.ShouldBe(2);
        }
        finally
        {
            if (clientId.HasValue)
            {
                using var scope = ServiceProvider.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<ICarContext>();
                await ctx.Clients.Where(x => x.Id == clientId.Value).ExecuteDeleteAsync(CancellationToken);
            }
        }
    }
}
