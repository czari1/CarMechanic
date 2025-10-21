using MediatR;

namespace Cars.Application.Services.UpdateService;

public sealed record UpdateServiceCommand(
    int ServiceId,
    string NewServiceName,
    string NewServiceDescription,
    decimal NewPrice)
    : IRequest;