using Cars.Application.Clients.AddClient;
using Cars.Application.Clients.DeleteClient;
using Cars.Application.Clients.DisplayAllCars;
using Cars.Application.Clients.GetCarById;
using Cars.Application.Clients.Models;
using Cars.Application.Clients.UpdateClient;
using Microsoft.AspNetCore.Mvc;

namespace Cars.Controllers.Clients;

[Route("/cars")]

public class CarController : ControllerBase
{
    [HttpGet("cars/{id:int}")]
    public async Task<ActionResult<CarDto>> GetCarById(int id, CancellationToken ct)
    {
        var cmd = new GetCarByIdCommand(id);
        return Ok();
    }

    [HttpGet("cars")]
    public async Task<ActionResult<IReadOnlyCollection<CarListDto>>> DisplayAllCars(CancellationToken ct)
    {
        var cmd = new DisplayAllCarsCommand();
        return Ok();
    }

    [HttpPost("car")] //Poprawiac wszystko
    public async Task<IActionResult> AddCar(
         int id,
         [FromBody] AddClientCommand cmd,
         CancellationToken ct)
    {
        return NoContent();
    }

    [HttpDelete("car/{id:int}")]
    public async Task<IActionResult> DeleteCar(int id, CancellationToken ct)
    {
        var cmd = new DeleteClientCommand(id);
        return NoContent();
    }

    [HttpPut("car/{id:int}")]
    public async Task<IActionResult> UpdateCar(
      int id,
      [FromBody] UpdateClientCommand cmd,
      CancellationToken ct)
    {
        return NoContent();
    }
}
