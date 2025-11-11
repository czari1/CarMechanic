using MediatR;

namespace Cars.Application.Clients.UpdateClient;

public sealed record UpdateClientCommand(
    int ClientId, //ma bycclientid
    string NewName, // dodac ClientId
    string NewSurname,
    string NewPhoneNumber)
: IRequest;

// Dodac pola od dodwania klienta
