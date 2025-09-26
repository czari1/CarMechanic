using Cars.Domain.Common.Entities;

namespace Cars.Domain.Entities;

public class Service : EntityBase
{
    //nazwy id 
    public int ServiceId { get; private set; }
    public string ServiceName { get; private set; }
    public string ServiceDescription { get; private set; }
    public int Price { get; private set; }
    //public ServiceState stateOfTheService {get; set private;} czy to w enumie oddzielnym pliku

    //konstruktory
    public Service(int serviceId, string serviceName, string serviceDescription, int price) 
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(serviceId);
        if (string.IsNullOrEmpty(serviceName)) throw new ArgumentNullException("Service name cannot be null");
        if (string.IsNullOrEmpty(serviceDescription)) throw new ArgumentNullException("Service description cannot be null");
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

        ServiceId = serviceId;
        ServiceName = serviceName;
        ServiceDescription = serviceDescription;
        Price = price;
    }
    protected Service() { }


}
