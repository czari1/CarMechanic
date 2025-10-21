using MediatR;

namespace Cars.Application.Services.DeleteService;

public sealed record DeleteServiceCommand(int Id)
: IRequest;
