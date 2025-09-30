using Cars.Application.Common;
using Cars.Infrastructure.Persistance.Const;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection;

namespace Cars.Domain.Entities;

public class CarsContext : DbContext , ICarContext
{
    public CarsContext(DbContextOptions<CarsContext> options) : base(options)
    {

    }
    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<Service> Services { get; set; } = null!;

    public IQueryable<Car> CarQuery => Set<Car>().AsNoTracking().AsQueryable();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"CarsConnectionString", x => x.MigrationsHistoryTable("__EFMigrationsHistory", DbConst.CARS_SCHEMA_NAME));
    } 
    
    // fluent validation entityframework (przeczytac)
    
}
