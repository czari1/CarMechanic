using Cars.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cars.Application.Clients.DisplayAllCars;

public sealed class DisplayAllCarsHandler(ICarContext context)
    : IRequestHandler<DisplayAllCarsCommand, IEnumerable<DisplayAllCarsModel>>
{
    public async Task<IEnumerable<DisplayAllCarsModel>> Handle(DisplayAllCarsCommand cmd, CancellationToken ct)
    {
        var cars = await context.CarQuery
            .AsNoTracking() //Czy Zostawic AsNoTrackibg
            .Where(c => !c.IsDeleted)
            .Select(c => new DisplayAllCarsModel(
                c.Id,
                c.Make,
                c.Model,
                c.Year,
                c.VIN))
            .ToListAsync(ct);
        return cars;

        //Warstwa modeli musi dosc do poprawy displaye
    }
}
