using Cars.Domain.Common.Interfaces;

namespace Cars.Domain.Common.Entities;

public class AggregateRoot : EntityBase, IAggregateRoot
{
    public bool IsDeleted { get; set; }

    //Domain driven design
    public virtual void Delete()
    {
        this.IsDeleted = true;
    }
}
