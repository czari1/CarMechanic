namespace Cars.Application.Clients.Models;

public sealed record CarListDto(
    int CarId,
    string Make,
    string Model,
    int Year,
    string Vin,
    string OwnerFullName,
    int OwnerId);
