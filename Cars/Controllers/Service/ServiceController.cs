using Cars.Application.Services.AddService;
using Cars.Application.Services.DeleteService;
using Cars.Application.Services.DisplayAllServices;
using Cars.Application.Services.GetServiceById;
using Cars.Application.Services.Models;
using Cars.Application.Services.UpdateService;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cars.Controllers.Service; //Przerobic caly plik zmiana na dto zmiast obiektow

[Route("/cars")]

public class ServiceController : ControllerBase
{
    private readonly IMediator _mediator;

    public ServiceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("service/{id:int}")]
    public async Task<ActionResult<ServiceDto>> GetServiceById(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetServiceByIdCommand(id), ct);

        if (result is null)
        {
            return NotFound($"Service with id {id} not found");
        }

        return Ok(result);
    }

    [HttpGet("services")]
    public async Task<ActionResult<IReadOnlyCollection<ServiceListDto>>> DisplayAllService(CancellationToken ct)
    {
        var result = await _mediator.Send(new DisplayAllServicesQuery(), ct);

        if (result is null)
        {
            return NotFound($"Services could not be found");
        }

        return Ok(result);
    }

    [HttpPost("service")] //Poprawiac wszystko
    public async Task<IActionResult> AddService(
         [FromBody] AddServiceCommand command,
         CancellationToken ct)
    {
        try
        {
            var serviceId = await _mediator.Send(command, ct);
            return CreatedAtAction(nameof(GetServiceById), new { id = serviceId }, serviceId);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Errors);
        }
    }

    [HttpDelete("service/{id:int}")]
    public async Task<IActionResult> DeleteService(int id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteServiceCommand(id));
        return NoContent();
    }

    [HttpPut("service/{id:int}")]
    public async Task<IActionResult> UpdateService(
      int id,
      [FromBody] UpdateServiceCommand command,
      CancellationToken ct)
    {
        if (id != command.ServiceId)
        {
            return BadRequest("Id for car not found");
        }

        await _mediator.Send(command, ct);
        return NoContent();
    }
}