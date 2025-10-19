using Bogus;
using Cars.Domain.Entities;

public sealed class ServiceBuilder
{
    private readonly Faker _faker;
    private int? _id;
    private string _serviceName;
    private string _serviceDescription;
    private decimal _price;
    private DateTime _serviceDate = DateTime.UtcNow;
    private DateTime _createdOn = DateTime.UtcNow;
    private DateTime _modifiedOn = DateTime.UtcNow;

    public ServiceBuilder(int? seed = null)
    {
        if (seed.HasValue)
        {
            Randomizer.Seed = new Random(seed.Value);
        }

        _faker = new Faker("en");
        _serviceName = _faker.Commerce.ProductName();
        _serviceDescription = _faker.Lorem.Sentence(10);
        _price = _faker.Finance.Amount(50, 2000);
    }

    public ServiceBuilder WithDefaults(
        string? serviceName = null,
        string? serviceDescription = null,
        decimal? price = null)
    {
        _serviceName = serviceName ?? "Oil Change";
        _serviceDescription = serviceDescription ?? "Standard oil change service";
        _price = price ?? 299.99m;
        return this;
    }

    public ServiceBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public ServiceBuilder WithServiceName(string serviceName)
    {
        _serviceName = serviceName;
        return this;
    }

    public ServiceBuilder WithServiceDescription(string serviceDescription)
    {
        _serviceDescription = serviceDescription;
        return this;
    }

    public ServiceBuilder WithPrice(decimal price)
    {
        _price = price;
        return this;
    }

    public ServiceBuilder WithServiceDate(DateTime serviceDate)
    {
        _serviceDate = serviceDate;
        return this;
    }

    public ServiceBuilder WithCreatedOn(DateTime createdOn)
    {
        _createdOn = createdOn;
        return this;
    }

    public ServiceBuilder WithModifiedOn(DateTime modifiedOn)
    {
        _modifiedOn = modifiedOn;
        return this;
    }

    public Service Build()
    {
        var entity = new Service(_serviceName, _serviceDescription, _price);

        if (_id.HasValue)
        {
            typeof(Service).GetProperty("Id")!.SetValue(entity, _id.Value);
        }

        typeof(Service).GetProperty("CreatedOn")!.SetValue(entity, _createdOn);
        typeof(Service).GetProperty("ModifiedOn")!.SetValue(entity, _modifiedOn);

        return entity;
    }
}