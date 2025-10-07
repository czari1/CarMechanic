using MediatR;

namespace Cars.Application.Clients.UpdateCar
{
    public sealed record UpdateCarCommand(
        int ClientId,
        int CarId,
        string NewMake,
        string NewModel,
        int NewYear)
    : IRequest;

    // Dodac pola od dodwania klienta

}
