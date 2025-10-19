using Cars.Application.Clients.Models;
using MediatR;

namespace Cars.Application.Clients.GetCarById;

public sealed record GetCarByIdCommand(int CarId)
    : IRequest<CarDto?>;