using MediatR;

namespace Cars.Application.Services.AddService;

public sealed record AddServiceCommand(
    int ServiceId,
    string ServiceName,
    string ServiceDescription,
    decimal Price,
    DateTime ServiceDate)
: IRequest<int>;

// Dodac pola od dodwania klienta
