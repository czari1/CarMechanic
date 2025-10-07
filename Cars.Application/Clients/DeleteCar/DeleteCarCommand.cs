using MediatR;

namespace Cars.Application.Clients.DeleteCar
{
    public sealed record DeleteCarCommand(
        int ClientId,
        int CarId)
    : IRequest;
}
