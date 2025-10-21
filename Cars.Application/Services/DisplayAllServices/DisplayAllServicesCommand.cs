using Cars.Application.Services.Models;
using MediatR;

namespace Cars.Application.Services.DisplayAllServices;

public sealed record DisplayAllServicesCommand()
: IRequest<IEnumerable<ServiceListDto>>;
