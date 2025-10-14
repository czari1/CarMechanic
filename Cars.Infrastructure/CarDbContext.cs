using Cars.Application.Common;
using Cars.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Cars.Infrastructure;

public class CarDbContext : DbContext, ICarContext
{
    public CarDbContext(DbContextOptions<CarDbContext> options)
        : base(options)
    {
    }

    public DbSet<Client> Clients { get; set; } = null!;

    public DbSet<Service> Services { get; set; } = null!;

    public IQueryable<Car> CarQuery => Set<Car>().AsNoTracking().AsQueryable();

    void IContext.Add<TAggregate>(TAggregate entity)
    {
        this.Add(entity);
    }

    void IContext.Delete<TAggregate>(TAggregate entity)
    {
        this.Remove(entity);
    }

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
