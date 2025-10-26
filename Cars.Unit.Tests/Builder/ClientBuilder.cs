using Bogus;
using Cars.Domain.Entities;
using Cars.Tests.Builders;

namespace Cars.Tests.Builders;

public sealed class ClientBuilder
{
    private readonly List<Car> _cars = new();
    private readonly Faker _faker;
    private int? _id;
    private string _name;
    private string _surname;
    private string _phoneNumber;
    private DateTime _createdOn = DateTime.UtcNow;
    private DateTime _modifiedOn = DateTime.UtcNow;
    private bool _isDeleted = false;

    public ClientBuilder(int? seed = null)
    {
        if (seed.HasValue)
        {
            Randomizer.Seed = new Random(seed.Value);
        }

        _faker = new Faker("en");
        _name = _faker.Name.FirstName();
        _surname = _faker.Name.LastName();
        _phoneNumber = _faker.Phone.PhoneNumber("##########").Substring(0, 9);
    }

    public ClientBuilder WithDefaults(
        string? name = null,
        string? surname = null,
        string? phoneNumber = null)
    {
        _name = name ?? "Jan";
        _surname = surname ?? "Kowalski";
        _phoneNumber = phoneNumber ?? "123456789";
        return this;
    }

    public ClientBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public ClientBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ClientBuilder WithSurname(string surname)
    {
        _surname = surname;
        return this;
    }

    public ClientBuilder WithPhoneNumber(string phoneNumber)
    {
        _phoneNumber = phoneNumber;
        return this;
    }

    public ClientBuilder WithCreatedOn(DateTime createdOn)
    {
        _createdOn = createdOn;
        return this;
    }

    public ClientBuilder WithModifiedOn(DateTime modifiedOn)
    {
        _modifiedOn = modifiedOn;
        return this;
    }

    public ClientBuilder WithDeleted(bool isDeleted = true)
    {
        _isDeleted = isDeleted;
        return this;
    }

    public ClientBuilder WithCar(
        string? make = null,
        string? model = null,
        int? year = null,
        string? vin = null)
    {
        var car = new CarBuilder(_faker.Random.Int())
            .WithMake(make ?? _faker.Vehicle.Manufacturer())
            .WithModel(model ?? _faker.Vehicle.Model())
            .WithYear(year ?? _faker.Date.Past(10).Year)
            .WithVin(vin ?? GenerateVin())
            .Build();

        _cars.Add(car);
        return this;
    }

    public ClientBuilder WithCars(int count)
    {
        for (int i = 0; i < count; i++)
        {
            WithCar();
        }
        return this;
    }

    public Client Build()
    {
        // Create entity without ID - let EF generate it
        var entity = new Client(_name, _surname, _phoneNumber);

        // Set ID via reflection if specified (for testing purposes only)
        if (_id.HasValue)
        {
            typeof(Client).GetProperty("Id")!.SetValue(entity, _id.Value);
        }

        typeof(Client).GetProperty("CreatedOn")!.SetValue(entity, _createdOn);
        typeof(Client).GetProperty("ModifiedOn")!.SetValue(entity, _modifiedOn);
        typeof(Client).GetProperty("IsDeleted")!.SetValue(entity, _isDeleted);

        foreach (var car in _cars)
        {
            entity.AddCar(car.Make, car.Model, car.Year, car.VIN);
        }

        return entity;
    }

    private string GenerateVin()
    {
        const string chars = "ABCDEFGHJKLMNPRSTUVWXYZ0123456789";
        return new string(Enumerable.Range(0, 17)
            .Select(_ => chars[_faker.Random.Int(0, chars.Length - 1)])
            .ToArray());
    }
}