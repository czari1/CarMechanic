using Cars.Application.Clients.Models;
using MediatR;

namespace Cars.Application.Clients.GetClientById;

public sealed record GetClientByIdCommand(int ClientId)
    : IRequest<ClientDto?>;