using MediatR;

namespace Cars.Application.Clients.UpdateClient;

public sealed record UpdateClientCommand(
    int NewId,
    string NewName, // dodac ClientId
    string NewSurname,
    string NewPhoneNumber)
: IRequest<int>;

// Dodac pola od dodwania klienta
