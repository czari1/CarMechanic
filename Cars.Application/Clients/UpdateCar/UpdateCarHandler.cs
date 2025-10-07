using Cars.Application.Common;
using MediatR;

namespace Cars.Application.Clients.UpdateCar;

public sealed class UpdateCarHandler(ICarContext context)
    : IRequestHandler<UpdateCarCommand>
{
    public async Task Handle(UpdateCarCommand cmd, CancellationToken ct)
    {
        var validator = new UpdateCarValidator();
        var result = await validator.ValidateAsync(cmd, ct);
        
        if (!result.IsValid) 
            return;

        var client = context.Clients.FirstOrDefault(c => c.Id == cmd.ClientId);
        if (client == null) 
            return;

        client.UpdateCar(cmd.CarId, cmd.NewMake, cmd.NewModel, cmd.NewYear); 
        //czy tez zmianiac Id i to samo w validator i command
        
        await context.SaveChangesAsync(ct);
    }
}