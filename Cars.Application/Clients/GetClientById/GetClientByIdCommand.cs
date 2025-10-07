using Cars.Domain.Entities;
using MediatR;

namespace Cars.Application.Clients.GetClientById;

public sealed record GetClientByIdCommand(int ClientId)
    : IRequest<Client?>;