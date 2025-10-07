using Cars.Application.Common;
using Cars.Domain.Entities;
using MediatR;

namespace Cars.Application.Clients.GetCarById;

public sealed record GetCarByIdHandler(ICarContext context)
    : IRequestHandler<GetCarByIdCommand, Car?>
{
    public async Task<Car?> Handle(GetCarByIdCommand cmd, CancellationToken ct)
    {
        var car = context.CarQuery.FirstOrDefault(c => c.Id == cmd.CarId);
        return await Task.FromResult(car);
    }
}