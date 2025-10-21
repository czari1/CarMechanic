using Cars.Application.Clients.AddClient;
using Cars.Application.Clients.DeleteClient;
using Cars.Application.Clients.DisplayAllClients;
using Cars.Application.Clients.GetClientById;
using Cars.Application.Clients.Models;
using Cars.Application.Clients.UpdateClient;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cars.Controllers.Clients; //Dodac mediatora do metod do kazdego pliku controller

[Route("/cars")]

public class ClientController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClientController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("client/{id:int}")]
    public async Task<ActionResult<ClientDto>> GetClientById(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetClientByIdCommand(id), ct);

        if (result == null)
        {
            return NotFound($"Client with id {id} not found");
        }

        return Ok(result);
    }

    [HttpGet("clients")]
    public async Task<ActionResult<IReadOnlyCollection<ClientListDto>>> DisplayAllClients(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DisplayAllClientsCommand());

        if (result == null)
        {
            return NotFound("Clients could not be found");
        }

        return Ok(result);
    }

    [HttpPost("client")] //Poprawiac wszystko
    public async Task<IActionResult> AddClient(
         [FromBody] AddClientCommand command,
         CancellationToken ct)
    {
        try
        {
            var clientId = await _mediator.Send(command, ct);
            return CreatedAtAction(nameof(GetClientById), new { id = clientId }, clientId);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Errors);
        }
    }

    [HttpDelete("client/{id:int}")]
    public async Task<IActionResult> DeleteClient(int id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteClientCommand(id));
        return NoContent();
    }

    [HttpPut("client/{id:int}")]
    public async Task<IActionResult> UpdateClient(
      int id,
      [FromBody] UpdateClientCommand command,
      CancellationToken ct)
    {
        if (id != command.ClientId)
        {
            return BadRequest("Id for client not found");
        }

        await _mediator.Send(command, ct);
        return NoContent();
    }
}