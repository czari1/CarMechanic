using MediatR;

namespace Cars.Application.Clients.DeleteClient
{
    public sealed record DeleteClientCommand(int Id)
    : IRequest;
}
