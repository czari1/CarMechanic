using Cars.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cars.Application.Clients.UpdateClient;

public sealed class UpdateClientHandler(ICarContext context)
: IRequestHandler<UpdateClientCommand>
{
    public async Task Handle(UpdateClientCommand cmd, CancellationToken ct)
    {
        var validator = new UpdateClientValidator();
        var result = await validator.ValidateAsync(cmd, ct);

        if (!result.IsValid)
        {
            return;
        }

        var existingClient = await context.Clients.FirstOrDefaultAsync(c => c.Id == cmd.ClientId);

        if (existingClient == null)
        {
            return;
        }

        existingClient.Update(cmd.ClientId, cmd.NewName, cmd.NewSurname, cmd.NewPhoneNumber);

        await context.SaveChangesAsync(ct);
    }
}
