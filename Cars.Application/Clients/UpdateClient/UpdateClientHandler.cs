using Cars.Application.Common;
using MediatR;

namespace Cars.Application.Clients.UpdateClient;

public sealed class UpdateClientHandler(ICarContext context)
: IRequestHandler<UpdateClientCommand, int>
{
    public async Task<int> Handle(UpdateClientCommand cmd, CancellationToken ct)
    {
        var validator = new UpdateClientValidator();
        var result = await validator.ValidateAsync(cmd, ct);

        if (!result.IsValid)
        {
            return 0;
        }

        var existingClient = await context.Clients.FindAsync(new object[] { cmd.NewId }, ct);
        if (existingClient is null)
        {
            return 0;
        }

        existingClient.Update(cmd.NewId, cmd.NewName, cmd.NewSurname, cmd.NewPhoneNumber);

        await context.SaveChangesAsync(ct);
        return existingClient.Id;
    }
}
