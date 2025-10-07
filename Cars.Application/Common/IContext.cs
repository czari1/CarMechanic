using Cars.Domain.Common.Interfaces;

namespace Cars.Application.Common;

public interface IContext 
{
    
    void Add<TAggregate>(TAggregate entity)
    where TAggregate : class, IAggregateRoot;

    void Delete<TAggregate>(TAggregate entity)
    where TAggregate : class, IAggregateRoot;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
