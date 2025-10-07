using Cars.Domain.Entities;
using MediatR;

namespace Cars.Application.Clients.GetCarById;

public sealed record GetCarByIdCommand(int CarId)
    : IRequest<Car?>;