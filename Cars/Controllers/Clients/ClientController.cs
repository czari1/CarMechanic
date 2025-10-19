using Cars.Application.Clients.AddClient;
using Cars.Application.Clients.DeleteClient;
using Cars.Application.Clients.Models;
using Cars.Application.Clients.UpdateClient;
using Cars.Application.Clients.DisplayAllClients;
using Cars.Application.Clients.GetClientById;
using Microsoft.AspNetCore.Mvc;

namespace Cars.Controllers.Clients; //Przerobic caly plik zmiana na dto zmiast obiektow

[Route("/cars")]

public class ClientController : ControllerBase
{
    [HttpGet("client/{id:int}")]
    public async Task<ActionResult<ClientDto>> GetClientById(int id, CancellationToken ct)
    {
        var cmd = new GetClientByIdCommand(id);
        return Ok();
    }

    [HttpGet("clients")]
    public async Task<ActionResult<IReadOnlyCollection<ClientListDto>>> DisplayAllClients(int id, CancellationToken ct)
    {
        var cmd = new DisplayAllClientsCommand();
        return Ok();
    }

    [HttpPost("client")] //Poprawiac wszystko
    public async Task<IActionResult> AddClient(
         int id,
         [FromBody] AddClientCommand cmd,
         CancellationToken ct)
    {
        return NoContent();
    }

    [HttpDelete("client/{id:int}")]
    public async Task<IActionResult> DeleteClient(int id, CancellationToken ct)
    {
        var cmd = new DeleteClientCommand(id);
        return NoContent();
    }

    [HttpPut("client/{id:int}")]
    public async Task<IActionResult> UpdateClient(
      int id,
      [FromBody] UpdateClientCommand cmd,
      CancellationToken ct)
    {
        return NoContent();
    }
}