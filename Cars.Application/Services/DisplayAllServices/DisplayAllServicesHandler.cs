using Cars.Application.Common;
using Cars.Application.Services.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cars.Application.Services.DisplayAllServices;

public sealed class DisplayAllServicesHandler(ICarContext context)
    : IRequestHandler<DisplayAllServicesQuery, IReadOnlyCollection<ServiceListDto>>
{
    public async Task<IReadOnlyCollection<ServiceListDto>> Handle(DisplayAllServicesQuery cmd, CancellationToken ct)
    {
        var services = await context.Services
            .AsNoTracking()
            .Select(s => new ServiceListDto(
                s.Id,
                s.ServiceName,
                s.ServiceDescription,
                s.Price,
                s.ServiceDate))
            .ToListAsync(ct);

        return services;
    }
}