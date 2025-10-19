using Cars.Application.Common;
using Cars.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Cars.Application.Services.AddService;

public sealed class AddServiceHandler(ICarContext context)
: IRequestHandler<AddServiceCommand, int>
{
    public async Task<int> Handle(AddServiceCommand cmd, CancellationToken ct)
    {
        var validator = new AddServiceValidator(context);
        var result = await validator.ValidateAsync(cmd, ct);

        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }

        var existingService = context.Services.FirstOrDefault(s => s.Id == cmd.ServiceId);

        if (existingService != null)
        {
            throw new ArgumentException($"Service with id {cmd.ServiceId} already exists.");
        }

        var entity = new Service(cmd.ServiceName, cmd.ServiceDescription, cmd.Price); 
        context.Services.Add(entity);

        await context.SaveChangesAsync(ct);

        var addedCar = client.Cars.FirstOrDefault(c => c.VIN == cmd.Vin);

        return addedCar?.Id ?? 0;
    }
}
