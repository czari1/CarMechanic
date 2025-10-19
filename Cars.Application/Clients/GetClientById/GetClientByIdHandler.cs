using Cars.Application.Clients.Models;
using Cars.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cars.Application.Clients.GetClientById;

public sealed record GetClientByIdHandler(ICarContext Context)
    : IRequestHandler<GetClientByIdCommand, ClientDto?>
{
    public async Task<ClientDto?> Handle(GetClientByIdCommand cmd, CancellationToken ct)
    {
        var client = await Context.Clients
            .AsNoTracking()
            .Where(client => client.Id == cmd.ClientId)
            .Select(client => new ClientDto(
                client.Id,
                client.Name,
                client.Surname,
                client.PhoneNumber,
                client.CreatedOn,
                client.ModifiedOn,
                client.Cars
                    .Select(car => new CarDto(
                        car.Id,
                        car.Make,
                        car.Model,
                        car.Year,
                        car.VIN,
                        car.Visits,
                        car.IsDeleted,
                        car.CreatedOn,
                        car.ModifiedOn))
                    .ToList()))
            .FirstOrDefaultAsync(ct);

        return client;
    }
}