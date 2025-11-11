using Cars.Application.Clients.AddCar;
using Cars.Application.Clients.DeleteCar;
using Cars.Application.Clients.DisplayAllCars;
using Cars.Application.Clients.GetCarById;
using Cars.Application.Clients.Models;
using Cars.Application.Clients.UpdateCar;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cars.Controllers.Cars;

[Route("/cars")]

public class CarController : ControllerBase
{
    private readonly IMediator _mediator;

    public CarController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("cars/{id:int}")]
    public async Task<ActionResult<CarDto>> GetCarById(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCarByIdCommand(id), ct);

        if (result is null)
        {
            return NotFound($"Car with id {id} not found");
        }

        return Ok(result);
    }

    [HttpGet("cars")]
    public async Task<ActionResult<IReadOnlyCollection<CarListDto>>> DisplayAllCars(CancellationToken ct)
    {
        var result = await _mediator.Send(new DisplayAllCarsCommand(), ct);

        if (result == null)
        {
            return NotFound($"Cars could not be found");
        }

        return Ok();
    }

    [HttpPost("car")] //Poprawiac wszystko
    public async Task<IActionResult> AddCar(
        [FromBody] AddCarCommand command,
        CancellationToken ct)
    {
        try
        {
            var carId = await _mediator.Send(command, ct);
            return CreatedAtAction(nameof(GetCarById), new { id = carId }, carId);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Errors);
        }
    }

    [HttpDelete("car/{id:int}")]
    public async Task<IActionResult> DeleteCar(int clientId, int carId, CancellationToken ct)
    {
        await _mediator.Send(new DeleteCarCommand(clientId, carId), ct);
        return NoContent();
    }

    [HttpPut("car/{id:int}")]
    public async Task<IActionResult> UpdateCar(
      int id,
      [FromBody] UpdateCarCommand command,
      CancellationToken ct)
    {
        if (id != command.CarId)
        {
            return BadRequest("Id for car not found");
        }

        await _mediator.Send(command, ct);
        return NoContent();
    }
}
