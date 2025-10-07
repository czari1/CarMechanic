using Cars.Domain.Entities;

namespace Cars.Application.Common;

public interface IQueryContext
{
    //tylko do odczytu
    IQueryable<Car> CarQuery { get; } //Robimy na kliencie wszytsko
}
