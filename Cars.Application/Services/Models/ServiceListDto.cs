namespace Cars.Application.Services.Models;

public sealed record ServiceListDto(
    int ServiceId,
    string ServiceName,
    string ServiceDescription,
    decimal Price,
    DateTime ServiceDate);
