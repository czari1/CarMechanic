using Cars.Application.Services.Models;
using Cars.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cars.Application.Services.GetServiceById;

public sealed record GetServiceByIdHandler(ICarContext Context)
    : IRequestHandler<GetServiceByIdCommand, ServiceDto?>
{
    public async Task<ServiceDto?> Handle(GetServiceByIdCommand cmd, CancellationToken ct)
    {
        var service = await Context.Services
            .AsNoTracking()
            .Where(s => s.Id == cmd.ServiceId)
            .Select(s => new ServiceDto(
                s.Id,
                s.ServiceName,
                s.ServiceDescription,
                s.Price,
                s.ServiceDate,
                s.CreatedOn,
                s.ModifiedOn))
            .FirstOrDefaultAsync(ct);

        return service;
    }
}