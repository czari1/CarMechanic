namespace Cars.Application.Clients.Models;

public sealed record CarDto(
    int CarId,
    string Make,
    string Model,
    int Year,
    string Vin,
    int Visits,
    bool IsDeleted,
    DateTime CreatedOn,
    DateTime ModifiedOn);
