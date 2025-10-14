using Cars.Application.Common;
using Cars.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Cars.Application.Clients.AddCar;

public sealed class AddCarHandler(ICarContext context)
: IRequestHandler<AddCarCommand, int>
{
    public async Task<int> Handle(AddCarCommand cmd, CancellationToken ct)
    {
        var validator = new AddCarValidator(context);
        var result = await validator.ValidateAsync(cmd, ct);

        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }

        var client = context.Clients.FirstOrDefault(c => c.Id == cmd.ClientId);

        if (client == null)
        {
            throw new KeyNotFoundException($"Client with id {cmd.ClientId} was not found.");
        }

        client.AddCar(cmd.Make, cmd.Model, cmd.Year, cmd.Vin);
        await context.SaveChangesAsync(ct);

        var addedCar = client.Cars.FirstOrDefault(c => c.VIN == cmd.Vin);

        return addedCar?.Id ?? 0;
    }
}
