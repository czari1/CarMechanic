using Cars.Application.Common;
using Cars.Domain.Entities;
using MediatR;

namespace Cars.Application.Clients.DisplayAllCars;

public sealed class DisplayAllCarsHandler(ICarContext context)
    : IRequestHandler<DisplayAllCarsCommand, IEnumerable<Car>>
{
    public async Task<IEnumerable<Car>> Handle(DisplayAllCarsCommand cmd, CancellationToken ct)
    {
        var cars = context.CarQuery.ToList(); //Warstwa modeli musi dosc do poprawy displaye
        return await Task.FromResult(cars);
    }
}
