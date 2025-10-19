using Cars.Application.Clients.Models;
using MediatR;

namespace Cars.Application.Clients.DisplayAllCars;

public sealed record DisplayAllCarsCommand()
: IRequest<IEnumerable<CarListDto>>;
