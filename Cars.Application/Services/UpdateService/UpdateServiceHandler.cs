using Cars.Application.Common;
using FluentValidation;
using MediatR;

namespace Cars.Application.Services.UpdateService;

public sealed class UpdateServiceHandler(ICarContext context)
    : IRequestHandler<UpdateServiceCommand>
{
    public async Task Handle(UpdateServiceCommand cmd, CancellationToken ct)
    {
        var validator = new UpdateServiceValidator();
        var result = await validator.ValidateAsync(cmd, ct);

        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }

        var service = context.Services.FirstOrDefault(s => s.Id == cmd.ServiceId);

        if (service != null) //Tak czy lepiej (service==null) i wtedy throw
        {
            service.Update(cmd.NewServiceName, cmd.NewServiceDescription, cmd.NewPrice);
            await context.SaveChangesAsync(ct);
        }
        else
        {
            throw new KeyNotFoundException($"Service with id {cmd.ServiceId} was not found.");
        }
    }
}