using Cars.Domain.Common.Entities;
using System.Net.Http.Headers;

namespace Cars.Domain.Entities;

public class Service : EntityBase
{
    //nazwy id 
    public int ServiceId { get; private set; }
    public string ServiceName { get; private set; }
    public string ServiceDescription { get; private set; }
    public decimal Price { get; private set; }
    public DateTime ServiceDate { get; private set; }
    //public ServiceState stateOfTheService {get; set private;} czy to w enumie oddzielnym pliku

    //konstruktory
    public Service(int serviceId, string serviceName, string serviceDescription, decimal price) 
    {
        ValidateServiceData(serviceId, serviceName, serviceDescription, price);

        ServiceId = serviceId;
        ServiceName = serviceName;
        ServiceDescription = serviceDescription;
        Price = price;
        ServiceDate = DateTime.UtcNow;
    }
    protected Service() { }

    private static void ValidateServiceData(int serviceId, string serviceName, string serviceDescription, decimal price)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(serviceId);
        
        if (string.IsNullOrEmpty(serviceName)) throw new ArgumentNullException("Service name cannot be null", nameof(serviceName));
        
        if (string.IsNullOrEmpty(serviceDescription)) throw new ArgumentNullException("Service description cannot be null", nameof(serviceDescription));
        
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price, nameof(price));
    }

    public void UpdateService(string newName, string newDescription, decimal newPrice)
    {
        if (!string.IsNullOrWhiteSpace(newName)) ServiceName = newName;

        if (!string.IsNullOrWhiteSpace(newDescription)) ServiceDescription = newDescription;

        if (newPrice >= 0) Price = newPrice;
    }


}
