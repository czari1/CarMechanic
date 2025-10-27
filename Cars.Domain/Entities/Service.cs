using Cars.Domain.Common.Entities;

namespace Cars.Domain.Entities;

public class Service : AggregateRoot
{
    //konstruktory
    public Service(string serviceName, string serviceDescription, decimal price)
    {
        ValidateServiceData(serviceName, serviceDescription, price);

        ServiceName = serviceName;
        ServiceDescription = serviceDescription;
        Price = price;
        ServiceDate = DateTime.UtcNow;
        CreatedOn = DateTime.UtcNow;
        ModifiedOn = DateTime.UtcNow;
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
    public void Update(string newServiceName, string newServiceDescription, decimal newPrice)
    {
        if (!string.IsNullOrWhiteSpace(newServiceName))
        {
            if (newServiceName.Length > 50)
            {
                throw new ArgumentException("Service name cannot exceed 50 characters", nameof(newServiceName));
            }

            ServiceName = newServiceName;
        }

        if (!string.IsNullOrWhiteSpace(newServiceDescription))
        {
            if (newServiceDescription.Length > 200)
            {
                throw new ArgumentException("Service description cannot exceed 200 characters", nameof(newServiceDescription));
            }

            ServiceDescription = newServiceDescription;
        }

        if (newPrice >= 0.0m)
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

        ArgumentOutOfRangeException.ThrowIfNegative(price, nameof(price));
    }
}
