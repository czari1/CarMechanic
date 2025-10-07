using MediatR;

namespace Cars.Application.Clients.AddCar
{
    public sealed record AddCarCommand(
        int ClientId,
        string Make, 
        string Model, 
        int Year,
        string Vin)
    : IRequest<int>;

    // Dodac pola od dodwania klienta

}
