namespace Cars.Application.Clients.Models;

public sealed record ClientListDto(
    int Id,
    string Name,
    string Surname,
    string PhoneNumber);
