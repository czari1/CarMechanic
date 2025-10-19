using Cars.Application.Clients.Models;
using Cars.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cars.Application.Clients.DisplayAllClients;

public sealed class DisplayAllClientsHandler(ICarContext context)
    : IRequestHandler<DisplayAllClientsCommand, IEnumerable<ClientListDto>>
{
    public async Task<IEnumerable<ClientListDto>> Handle(DisplayAllClientsCommand cmd, CancellationToken ct)
    {
        var clients = await context.Clients
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .Select(c => new ClientListDto(
                c.Id,
                c.Name,
                c.Surname,
                c.PhoneNumber))
            .ToListAsync(ct);

        return clients;
    }
}
