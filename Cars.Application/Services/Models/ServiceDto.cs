namespace Cars.Application.Services.Models;

public sealed record ServiceDto(
    int ServiceId,
    string ServiceName,
    string ServiceDescription,
    decimal Price,
    DateTime ServiceDate,
    DateTime CreatedOn,
    DateTime ModifiedOn);
