using Cars.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.Application.Common;

public interface ICarContext : IQueryContext , IContext
{
    DbSet<Client> Clients { get; }
    DbSet<Service> Services { get; }
}
