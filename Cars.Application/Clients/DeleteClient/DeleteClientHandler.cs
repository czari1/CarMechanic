using Cars.Application.Clients.AddClient;
using Cars.Application.Common;
using Cars.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.Application.Clients.DeleteClient
{
    public sealed class DeleteClintHandler(ICarContext context)
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
