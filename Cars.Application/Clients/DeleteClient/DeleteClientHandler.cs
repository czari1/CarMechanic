using Cars.Application.Common;
using MediatR;


namespace Cars.Application.Clients.DeleteClient
{
    public sealed class DeleteClientHandler(ICarContext context)
    : IRequestHandler<DeleteClientCommand>
    {
        public async Task Handle(DeleteClientCommand cmd, CancellationToken ct)
        {
            var entity = context.Clients.FirstOrDefault(c => c.Id == cmd.Id);
            if (entity == null) return; // Do poprawy
            context.Delete(entity);
            await context.SaveChangesAsync(ct);
        }
    }
}
