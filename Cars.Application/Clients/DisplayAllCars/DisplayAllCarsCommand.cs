using Cars.Domain.Entities;
using MediatR;

namespace Cars.Application.Clients.DisplayAllCars;

public sealed record DisplayAllCarsCommand()
: IRequest<IEnumerable<Car>>;
