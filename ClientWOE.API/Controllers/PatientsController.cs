using ClientWOE.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClientWOE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly WoeDbContext _db;

    public PatientsController(WoeDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetPatients([FromQuery] string? search, [FromQuery] int? clientId)
    {
        var query = _db.Patients.AsQueryable();

        if (clientId.HasValue)
            query = query.Where(x => x.ClientId == clientId.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.PatientName.Contains(search) || x.PatientCode.Contains(search));

        var patients = await query
            .OrderBy(x => x.PatientName)
            .Take(50)
            .Select(x => new
            {
                x.PatientId,
                x.PatientCode,
                x.PatientName,
                x.Age,
                x.Gender,
                x.MobileNumber,
                x.ClientId
            })
            .ToListAsync();

        return Ok(patients);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPatient(int id)
    {
        var patient = await _db.Patients.FindAsync(id);
        if (patient == null)
            return NotFound(new { message = "Patient not found." });

        return Ok(patient);
    }
}
