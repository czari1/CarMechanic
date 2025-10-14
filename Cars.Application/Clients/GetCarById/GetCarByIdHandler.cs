using Cars.Application.Common;
using Cars.Domain.Entities;
using MediatR;

namespace Cars.Application.Clients.GetCarById;

public sealed record GetCarByIdHandler(ICarContext Context)
    : IRequestHandler<GetCarByIdCommand, Car?>
{
    public async Task<Car?> Handle(GetCarByIdCommand cmd, CancellationToken ct)
    {
        var car = Context.CarQuery.FirstOrDefault(c => c.Id == cmd.CarId);

        if (car == null)
        {
            throw new KeyNotFoundException($"Car with id {cmd.CarId} was not found.");
        }

        return await Task.FromResult(car);
    }
}