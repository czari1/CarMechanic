using Cars.Application.Clients.Models;
using Cars.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cars.Application.Clients.DisplayAllCars;

public sealed class DisplayAllCarsHandler(ICarContext context)
    : IRequestHandler<DisplayAllCarsCommand, IEnumerable<CarListDto>>
{
    public async Task<IEnumerable<CarListDto>> Handle(DisplayAllCarsCommand cmd, CancellationToken ct)
    {
        var cars = await context.Clients
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .SelectMany(client => client.Cars
                .Where(car => !car.IsDeleted)
                .Select(car => new CarListDto(
                    car.Id,
                    car.Make,
                    car.Model,
                    car.Year,
                    car.VIN,
                    $"{client.Name} {client.Surname}",
                    client.Id)))
            .ToListAsync(ct);

        return cars;
    }
}
