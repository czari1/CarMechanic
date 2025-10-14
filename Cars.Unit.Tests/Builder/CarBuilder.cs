using Bogus;
using Cars.Domain.Entities;

public sealed class CarBuilder
{
    private readonly Faker _faker;
    private int? _id;
    private string _make;
    private string _model;
    private int _year;
    private string _vin;
    private int _visits = 0;
    private DateTime _createdOn = DateTime.UtcNow;
    private DateTime _modifiedOn = DateTime.UtcNow;
    private bool _isDeleted = false;

    public CarBuilder(int? seed = null)
    {
        if (seed.HasValue)
        {
            Randomizer.Seed = new Random(seed.Value);
        }

        _faker = new Faker("en");
        _make = _faker.Vehicle.Manufacturer();
        _model = _faker.Vehicle.Model();
        _year = _faker.Date.Past(10).Year;
        _vin = GenerateVin();
    }

    public CarBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public CarBuilder WithMake(string make)
    {
        _make = make;
        return this;
    }

    public CarBuilder WithModel(string model)
    {
        _model = model;
        return this;
    }

    public CarBuilder WithYear(int year)
    {
        _year = year;
        return this;
    }

    public CarBuilder WithVin(string vin)
    {
        _vin = vin;
        return this;
    }

    public CarBuilder WithVisits(int visits)
    {
        _visits = visits;
        return this;
    }

    public CarBuilder WithCreatedOn(DateTime createdOn)
    {
        _createdOn = createdOn;
        return this;
    }

    public CarBuilder WithModifiedOn(DateTime modifiedOn)
    {
        _modifiedOn = modifiedOn;
        return this;
    }

    public CarBuilder WithDeleted(bool isDeleted = true)
    {
        _isDeleted = isDeleted;
        return this;
    }

    public Car Build()
    {
        var entity = new Car(_make, _model, _year, _vin);

        if (_id.HasValue)
        {
            entity.Id = _id.Value;
        }

        entity.CreatedOn = _createdOn;
        entity.ModifiedOn = _modifiedOn;
        entity.IsDeleted = _isDeleted;

        for (int i = 0; i < _visits; i++)
        {
            entity.IncrementVisits();
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