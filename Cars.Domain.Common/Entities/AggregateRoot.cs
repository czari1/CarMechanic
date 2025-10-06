using Cars.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
