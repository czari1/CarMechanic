using Bogus;
using Cars.Application.Common;
using Cars.Domain.Entities;

namespace Cars.Integration.Application.Builder;

public sealed class ClientBuilder
{
    private readonly Faker _faker;
    private readonly ICarContext _context;
    private readonly List<Action<Client>> _carActions = new();
    private string _name;
    private string _surname;
    private string _phoneNumber;

    public ClientBuilder(ICarContext context, int? seed = null)
    {
        _context = context;

        if (seed.HasValue)
        {
            Randomizer.Seed = new Random(seed.Value);
        }

        _faker = new Faker("en"); // Initialize _faker BEFORE calling WithDefaults

        WithDefaults();
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

    public ClientBuilder WithCar(
        string? make = null,
        string? model = null,
        int? year = null,
        string? vin = null)
    {
        _carActions.Add(client =>
        {
            var carMake = make ?? _faker.Vehicle.Manufacturer();
            var carModel = model ?? _faker.Vehicle.Model();
            var carYear = year ?? _faker.Date.Past(10).Year;
            var carVin = vin ?? GenerateVin();
            client.AddCar(carMake, carModel, carYear, carVin);
        });
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

    public async Task<Client> Build(CancellationToken ct = default)
    {
        var client = new Client(_name, _surname, _phoneNumber);

        foreach (var carAction in _carActions)
        {
            carAction(client);
        }

        _context.Add(client);
        await _context.SaveChangesAsync(ct);

        return client;
    }

    private string GenerateVin()
    {
        const string chars = "ABCDEFGHJKLMNPRSTUVWXYZ0123456789";
        var vin = new char[17];
        for (int i = 0; i < 17; i++)
        {
            vin[i] = chars[_faker.Random.Int(0, chars.Length - 1)];
        }
        return new string(vin);
    }
}