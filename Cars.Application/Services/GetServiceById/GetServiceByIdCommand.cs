using Cars.Application.Services.Models;
using MediatR;

namespace Cars.Application.Services.GetServiceById;

public sealed record GetServiceByIdCommand(int ServiceId)
: IRequest<ServiceDto?>;
