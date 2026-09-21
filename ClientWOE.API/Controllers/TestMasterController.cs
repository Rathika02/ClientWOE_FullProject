using ClientWOE.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClientWOE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestMasterController : ControllerBase
{
    private readonly WoeDbContext _db;

    public TestMasterController(WoeDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetTests([FromQuery] string? search)
    {
        var query = _db.TestMasters.Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.TestName.Contains(search) || x.TestCode.Contains(search));

        var tests = await query
            .OrderBy(x => x.TestName)
            .Take(50)
            .Select(x => new
            {
                x.TestId,
                x.TestCode,
                x.TestName,
                x.Rate
            })
            .ToListAsync();

        return Ok(tests);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTest(int id)
    {
        var test = await _db.TestMasters.FindAsync(id);
        if (test == null)
            return NotFound(new { message = "Test not found." });

        return Ok(test);
    }
}
