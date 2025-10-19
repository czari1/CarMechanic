using Cars.Application.Clients.Models;
using MediatR;

namespace Cars.Application.Clients.DisplayAllClients;

public sealed record DisplayAllClientsCommand()
: IRequest<IEnumerable<ClientListDto>>;
