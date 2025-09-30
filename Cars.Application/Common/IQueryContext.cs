using Cars.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.Application.Common
{
    public interface IQueryContext
    {
        //tylko do odczytu
        IQueryable<Car> CarQuery { get; } //Robimy na kliencie wszytsko
    }
}
