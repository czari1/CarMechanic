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
        var validator = new AddServiceValidator();
        var result = await validator.ValidateAsync(cmd, ct);

        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }

        var entity = new Service(cmd.ServiceName, cmd.ServiceDescription, cmd.Price);
        context.Services.Add(entity);
        await context.SaveChangesAsync(ct);

        return entity.Id;
    }
}
