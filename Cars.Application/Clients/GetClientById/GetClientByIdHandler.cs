using Cars.Application.Common;
using Cars.Domain.Entities;
using MediatR;

namespace Cars.Application.Clients.GetClientById;

public sealed record GetClientByIdHandler(ICarContext context)
    : IRequestHandler<GetClientByIdCommand, Client?>
{
    public async Task<Client?> Handle(GetClientByIdCommand cmd, CancellationToken ct)
    {
        var client = context.Clients.FirstOrDefault(c => c.Id == cmd.ClientId);
        return await Task.FromResult(client);
    }
}