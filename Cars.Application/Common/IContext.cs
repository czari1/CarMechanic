using Cars.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.Application.Common
{
    public interface IContext 
    {
        
        void Add<TAggregate>(TAggregate entity)
          where TAggregate : class, IAggregateRoot;

        void Delete<TAggregate>(TAggregate entity)
        where TAggregate : class, IAggregateRoot;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
