using Cars.Application.Common;
using MediatR;

namespace Cars.Application.Clients.DeleteCar;

public sealed class DeleteCarHandler(ICarContext context)
: IRequestHandler<DeleteCarCommand>
{
    public async Task Handle(DeleteCarCommand cmd, CancellationToken ct)
    {
        var client = context.Clients.FirstOrDefault(c => c.Id == cmd.ClientId);

        if (client == null)
        {
            throw new KeyNotFoundException($"Car with id {cmd.ClientId} was not found.");
        }

        client.RemoveCar(cmd.CarId);

        await context.SaveChangesAsync(ct);
    }
}
