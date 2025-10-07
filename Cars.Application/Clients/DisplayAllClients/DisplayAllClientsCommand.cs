using Cars.Domain.Entities;
using MediatR;

namespace Cars.Application.Clients.DisplayAllClients
{
    public sealed record DisplayAllClientsCommand()
    : IRequest<IEnumerable<Client>>;
}
