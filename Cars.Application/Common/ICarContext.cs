using Cars.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cars.Application.Common;

public interface ICarContext : IQueryContext , IContext
{
    DbSet<Client> Clients { get; }
    DbSet<Service> Services { get; }
}
