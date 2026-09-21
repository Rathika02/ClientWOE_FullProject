using ClientWOE.API.Data;
using ClientWOE.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClientWOE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly WoeDbContext _db;

    public ClientsController(WoeDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetClients()
    {
        var clients = await _db.Clients
            .Where(x => x.IsActive)
            .OrderBy(x => x.ClientName)
            .Select(x => new { x.ClientId, x.ClientCode, x.ClientName })
            .ToListAsync();

        return Ok(clients);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetClient(int id)
    {
        var client = await _db.Clients.FindAsync(id);
        if (client == null)
            return NotFound(new { message = "Client not found." });

        return Ok(client);
    }

    [HttpPost]
    public async Task<IActionResult> CreateClient(Client client)
    {
        if (await _db.Clients.AnyAsync(x => x.ClientCode == client.ClientCode))
            return Conflict(new { message = "Client code already exists." });

        client.ClientId = 0;
        _db.Clients.Add(client);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetClient), new { id = client.ClientId }, client);
    }
}
