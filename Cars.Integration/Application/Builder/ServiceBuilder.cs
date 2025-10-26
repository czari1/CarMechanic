using Bogus;
using Cars.Application.Common;
using Cars.Domain.Entities;

namespace Cars.Integration.Application.Builder;

public sealed class ServiceBuilder
{
    private readonly Faker _faker;
    private readonly ICarContext _context;
    private string _serviceName;
    private string _serviceDescription;
    private decimal _price;

    public ServiceBuilder(ICarContext context, int? seed = null)
    {
        _context = context;

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

    public async Task<Service> Build(CancellationToken ct = default)
    {
        var service = new Service(_serviceName, _serviceDescription, _price);

        _context.Add(service);
        await _context.SaveChangesAsync(ct);

        return service;
    }
}