using Cars.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cars.Application.Clients.DisplayAllClients;

public sealed class DisplayAllClientsHandler(ICarContext context)
    : IRequestHandler<DisplayAllClientsCommand, IEnumerable<DisplayAllClientsModel>>
{
    public async Task<IEnumerable<DisplayAllClientsModel>> Handle(DisplayAllClientsCommand cmd, CancellationToken ct)
    {
        var clients = await context.Clients
            .AsNoTracking()
            .Select(c => new DisplayAllClientsModel(
                c.Id,
                c.Name,
                c.Surname,
                c.PhoneNumber))
            .ToListAsync(ct);

        return clients;
    }
}
