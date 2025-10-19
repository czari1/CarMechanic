namespace Cars.Application.Clients.Models;

public sealed record ClientDto(
    int Id,
    string Name,
    string Surname,
    string PhoneNumber,
    DateTime CreatedOn,
    DateTime ModifiedOn,
    IReadOnlyCollection<CarDto> Cars);