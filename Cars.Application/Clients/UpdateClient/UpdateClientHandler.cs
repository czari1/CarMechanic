using Cars.Application.Common;
using Cars.Domain.Entities;
using MediatR;

namespace Cars.Application.Clients.UpdateClient;

public sealed class UpdateClientHandler(ICarContext context)
: IRequestHandler<UpdateClientCommand, int>
{
    public async Task<int> Handle(UpdateClientCommand cmd, CancellationToken ct)
    {
        var entity = new Client(cmd.NewName, cmd.NewSurname, cmd.NewPhoneNumber);
        var validator = new UpdateClientValidator();
        var result = await validator.ValidateAsync(cmd, ct);
        if (result.IsValid) return 0;
        context.Add(entity);
        await context.SaveChangesAsync(ct);
        return entity.Id;
    }
}
