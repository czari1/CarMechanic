using Cars.Application.Common;
using Cars.Domain.Entities;
using Cars.Infrastructure.Persistance.Const;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection;

namespace Cars.Infrastructure;

public class CarDbContext : DbContext , ICarContext
{
    public CarDbContext(DbContextOptions<CarDbContext> options) : base(options)
    {

    }
    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<Service> Services { get; set; } = null!;

    void IContext.Add<TAggregate>(TAggregate entity)
    {
        base.Add(entity);
    }
    // Spytac sie o te dwie metody
    void IContext.Delete<TAggregate>(TAggregate entity)
    {
        base.Remove(entity);
    }

    public IQueryable<Car> CarQuery => Set<Car>().AsNoTracking().AsQueryable();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseSqlServer(@"CarsConnectionString", x => x.MigrationsHistoryTable("__EFMigrationsHistory", DbConst.CARS_SCHEMA_NAME));
    //} 
    
    
    
}
