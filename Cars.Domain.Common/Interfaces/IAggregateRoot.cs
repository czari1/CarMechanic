namespace Cars.Domain.Common.Interfaces;

public interface IAggregateRoot : ISoftDeleteTable
{
    void AddCar(string make, string model, int year, string vin);
}
