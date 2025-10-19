using Cars.Domain.Common.Entities;

namespace Cars.Domain.Entities;

public class Service : EntityBase
{
    //konstruktory
    public Service(string serviceName, string serviceDescription, decimal price)
    {
        ValidateServiceData(serviceName, serviceDescription, price);

        ServiceName = serviceName;
        ServiceDescription = serviceDescription;
        Price = price;
        ServiceDate = DateTime.UtcNow;
    }

    protected Service()
    {
        ServiceName = string.Empty;
        ServiceDescription = string.Empty;
    }

    //nazwy
    public string ServiceName { get; private set; }

    public string ServiceDescription { get; private set; }

    public decimal Price { get; private set; }

    public DateTime ServiceDate { get; private set; }

    //public ServiceState stateOfTheService {get; set private;} czy to w enumie oddzielnym pliku

    public void AddService(string serviceName, string serviceDescription, decimal price)
    {
        Service newService = new Service(serviceName, serviceDescription, price);
    }

    public void UpdateService(string newName, string newDescription, decimal newPrice)
    {
        if (!string.IsNullOrWhiteSpace(newName))
        {
            ServiceName = newName;
        }

        if (!string.IsNullOrWhiteSpace(newDescription))
        {
            ServiceDescription = newDescription;
        }

        if (newPrice >= 0)
        {
            Price = newPrice;
        }
    }

    private static void ValidateServiceData(string serviceName, string serviceDescription, decimal price)
    {
        if (string.IsNullOrEmpty(serviceName))
        {
            throw new ArgumentNullException("Service name cannot be null", nameof(serviceName));
        }

        if (string.IsNullOrEmpty(serviceDescription))
        {
            throw new ArgumentNullException("Service description cannot be null", nameof(serviceDescription));
        }

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price, nameof(price));
    }
}
