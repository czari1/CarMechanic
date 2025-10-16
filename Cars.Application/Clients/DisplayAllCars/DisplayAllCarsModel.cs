namespace Cars.Application.Clients.DisplayAllCars;

public sealed record DisplayAllCarsModel(
    int Id,
    string Make,
    string Model,
    int Year,
    string Vin);