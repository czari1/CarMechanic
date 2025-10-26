using Bogus;
using Cars.Application.Common;
using Cars.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cars.Integration.Application.Builder;

public sealed class CarBuilder
{
    private readonly Faker _faker;
    private readonly ICarContext _context;
    private string _make;
    private string _model;
    private int _year;
    private string _vin;

    public CarBuilder(ICarContext context, int? seed = null)
    {
        _context = context;

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

    public CarBuilder WithDefaults(
        string? make = null,
        string? model = null,
        int? year = null,
        string? vin = null)
    {
        _make = make ?? "Toyota";
        _model = model ?? "Corolla";
        _year = year ?? DateTime.Now.Year;
        _vin = vin ?? "1HGBH41JXMN109186";
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

    public async Task<Car> Build(int clientId, CancellationToken ct = default)
    {
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Id == clientId, ct);

        if (client == null)
        {
            throw new InvalidOperationException($"Client with id {clientId} not found");
        }

        client.AddCar(_make, _model, _year, _vin);
        await _context.SaveChangesAsync(ct);

        var car = client.Cars.FirstOrDefault(c => c.VIN == _vin);
        if (car == null)
        {
            throw new InvalidOperationException("Failed to add car to client");
        }

        return car;
    }

    private string GenerateVin()
    {
        const string chars = "ABCDEFGHJKLMNPRSTUVWXYZ0123456789";
        return new string(Enumerable.Range(0, 17)
            .Select(_ => chars[_faker.Random.Int(0, chars.Length - 1)])
            .ToArray());
    }
}