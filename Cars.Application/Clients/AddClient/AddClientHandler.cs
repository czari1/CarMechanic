using Cars.Application.Common;
using Cars.Domain.Entities;
using MediatR;

namespace Cars.Application.Clients.AddClient;

public sealed class AddClientHandler(ICarContext context)
: IRequestHandler<AddClientCommand, int>
{ 
    public async Task<int> Handle(AddClientCommand cmd, CancellationToken ct)
    {
        var entity = new Client (cmd.Name, cmd.Surname, cmd.PhoneNumber);
        var validator = new AddClientValidator();
        var result = await validator.ValidateAsync(cmd, ct);
        if (result.IsValid) return 0; 
        context.Add(entity);
        await context.SaveChangesAsync(ct);
        return entity.Id;
    }
}
