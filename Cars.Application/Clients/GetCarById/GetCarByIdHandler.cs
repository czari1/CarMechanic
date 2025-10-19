using Cars.Application.Clients.Models;
using Cars.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cars.Application.Clients.GetCarById;

public sealed record GetCarByIdHandler(ICarContext Context)
    : IRequestHandler<GetCarByIdCommand, CarDto?>
{
    public async Task<CarDto?> Handle(GetCarByIdCommand cmd, CancellationToken ct)
    {
        var car = await Context.Clients
            .AsNoTracking()
            .SelectMany(client => client.Cars
                .Where(c => c.Id == cmd.CarId)
                .Select(c => new CarDto(
                    c.Id,
                    c.Make,
                    c.Model,
                    c.Year,
                    c.VIN,
                    c.Visits,
                    c.IsDeleted,
                    c.CreatedOn,
                    c.ModifiedOn))
                .ToList())
        .FirstOrDefaultAsync(ct);

        return car;
    }
}