using Cars.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cars.Application.Services.DeleteService;

public sealed class DeleteServiceHandler(ICarContext context)
    : IRequestHandler<DeleteServiceCommand>
{
    public async Task Handle(DeleteServiceCommand cmd, CancellationToken ct)
    {
        var entity = await context.Services
            .FirstOrDefaultAsync(s => s.Id == cmd.Id);
        if (entity != null)
        {
            context.Services.Remove(entity);
            await context.SaveChangesAsync(ct);
        }
        else
        {
            throw new KeyNotFoundException($"Service with id {cmd.Id} not found");
        }
    }
}