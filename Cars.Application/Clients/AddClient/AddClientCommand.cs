using MediatR;

namespace Cars.Application.Clients.AddClient;

public sealed record AddClientCommand(
    string Name,
    string Surname,
    string PhoneNumber)
: IRequest<int>;

// Dodac pola od dodwania klienta
