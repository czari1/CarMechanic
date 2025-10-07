using Cars.Application.Common;
using Cars.Domain.Entities;
using MediatR;

namespace Cars.Application.Clients.DisplayAllClients;

public sealed class DisplayAllClientsHandler(ICarContext context)
    : IRequestHandler<DisplayAllClientsCommand, IEnumerable<Client>>
{
    public async Task<IEnumerable<Client>> Handle(DisplayAllClientsCommand cmd, CancellationToken ct)
    {
        var clients = context.Clients.ToList();
        return await Task.FromResult(clients);
    }
}
