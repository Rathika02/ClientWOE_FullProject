using ClientWOE.API.Data;
using ClientWOE.API.DTOs;
using ClientWOE.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClientWOE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkOrdersController : ControllerBase
{
    private readonly WoeDbContext _db;

    public WorkOrdersController(WoeDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetWorkOrders([FromQuery] int? clientId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var query = _db.WorkOrders
            .Include(x => x.Client)
            .Include(x => x.Patient)
            .AsQueryable();

        if (clientId.HasValue)
            query = query.Where(x => x.ClientId == clientId.Value);

        if (from.HasValue)
            query = query.Where(x => x.OrderDate >= from.Value);

        if (to.HasValue)
            query = query.Where(x => x.OrderDate < to.Value.Date.AddDays(1));

        var result = await query
            .OrderByDescending(x => x.OrderDate)
            .Take(100)
            .Select(x => new
            {
                x.WorkOrderId,
                x.WoeNumber,
                x.OrderDate,
                ClientName = x.Client!.ClientName,
                PatientName = x.Patient!.PatientName,
                x.TotalAmount,
                x.Status
            })
            .ToListAsync();

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetWorkOrder(int id)
    {
        var order = await _db.WorkOrders
            .Include(x => x.Client)
            .Include(x => x.Patient)
            .Include(x => x.TestDetails)
                .ThenInclude(x => x.TestMaster)
            .FirstOrDefaultAsync(x => x.WorkOrderId == id);

        if (order == null)
            return NotFound(new { message = "Work order not found." });

        return Ok(ToResponse(order));
    }

    [HttpPost]
    public async Task<IActionResult> CreateWorkOrder(CreateWorkOrderRequest request)
    {
        if (request.Tests == null || request.Tests.Count == 0)
            return BadRequest(new { message = "At least one test must be selected." });

        var client = await _db.Clients
            .FirstOrDefaultAsync(x => x.ClientId == request.ClientId && x.IsActive);

        if (client == null)
            return BadRequest(new { message = "Selected client does not exist or is inactive." });

        var testIds = request.Tests.Select(x => x.TestId).Distinct().ToList();

        if (testIds.Count != request.Tests.Count)
            return BadRequest(new { message = "Duplicate tests are not allowed." });

        var tests = await _db.TestMasters
            .Where(x => testIds.Contains(x.TestId) && x.IsActive)
            .ToListAsync();

        if (tests.Count != testIds.Count)
            return BadRequest(new { message = "One or more selected tests do not exist or are inactive." });

        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            var patientCode = await GeneratePatientCodeAsync();

            var patient = new Patient
            {
                PatientCode = patientCode,
                ClientId = client.ClientId,
                PatientName = request.PatientName.Trim(),
                Age = request.Age,
                Gender = request.Gender,
                MobileNumber = request.MobileNumber.Trim()
            };

            _db.Patients.Add(patient);
            await _db.SaveChangesAsync();

            var woeNumber = await GenerateWoeNumberAsync();

            var order = new WorkOrder
            {
                WoeNumber = woeNumber,
                ClientId = client.ClientId,
                PatientId = patient.PatientId,
                OrderDate = DateTime.Now,
                Status = "Saved"
            };

            foreach (var item in request.Tests)
            {
                var test = tests.First(x => x.TestId == item.TestId);
                var amount = test.Rate * item.Quantity;

                order.TestDetails.Add(new WorkOrderTestDetail
                {
                    TestId = test.TestId,
                    Quantity = item.Quantity,
                    Rate = test.Rate,
                    Amount = amount
                });

                order.TotalAmount += amount;
            }

            _db.WorkOrders.Add(order);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            var saved = await _db.WorkOrders
                .Include(x => x.Client)
                .Include(x => x.Patient)
                .Include(x => x.TestDetails)
                    .ThenInclude(x => x.TestMaster)
                .FirstAsync(x => x.WorkOrderId == order.WorkOrderId);

            return CreatedAtAction(nameof(GetWorkOrder), new { id = saved.WorkOrderId }, ToResponse(saved));
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<string> GeneratePatientCodeAsync()
    {
        var last = await _db.Patients
            .OrderByDescending(x => x.PatientId)
            .Select(x => x.PatientId)
            .FirstOrDefaultAsync();

        return $"PAT-{last + 1:000000}";
    }

    private async Task<string> GenerateWoeNumberAsync()
    {
        var datePart = DateTime.Now.ToString("yyyyMMdd");
        var prefix = $"WOE-{datePart}-";

        var numbers = await _db.WorkOrders
            .Where(x => x.WoeNumber.StartsWith(prefix))
            .Select(x => x.WoeNumber)
            .ToListAsync();

        var next = 1;

        if (numbers.Count > 0)
        {
            var parsed = numbers
                .Select(x => int.TryParse(x.Substring(prefix.Length), out var n) ? n : 0)
                .Max();

            next = parsed + 1;
        }

        return $"{prefix}{next:0000}";
    }

    private static WorkOrderResponse ToResponse(WorkOrder order)
    {
        return new WorkOrderResponse
        {
            WorkOrderId = order.WorkOrderId,
            WoeNumber = order.WoeNumber,
            OrderDate = order.OrderDate,
            ClientName = order.Client?.ClientName ?? "",
            PatientCode = order.Patient?.PatientCode ?? "",
            PatientName = order.Patient?.PatientName ?? "",
            Age = order.Patient?.Age ?? 0,
            Gender = order.Patient?.Gender.ToString() ?? "",
            MobileNumber = order.Patient?.MobileNumber ?? "",
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            Tests = order.TestDetails.Select(x => new WorkOrderTestResponse
            {
                TestId = x.TestId,
                TestName = x.TestMaster?.TestName ?? "",
                Rate = x.Rate,
                Quantity = x.Quantity,
                Amount = x.Amount
            }).ToList()
        };
    }
}
