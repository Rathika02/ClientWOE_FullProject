using System.ComponentModel.DataAnnotations;
using ClientWOE.API.Models;

namespace ClientWOE.API.DTOs;

public class WorkOrderTestRequest
{
    [Range(1, int.MaxValue)]
    public int TestId { get; set; }

    [Range(1, 1000)]
    public int Quantity { get; set; } = 1;
}

public class CreateWorkOrderRequest
{
    [Range(1, int.MaxValue)]
    public int ClientId { get; set; }

    [Required, MaxLength(100)]
    public string PatientName { get; set; } = string.Empty;

    [Range(0, 120)]
    public int Age { get; set; }

    public Gender Gender { get; set; }

    [Required, Phone, MaxLength(15)]
    public string MobileNumber { get; set; } = string.Empty;

    [MinLength(1)]
    public List<WorkOrderTestRequest> Tests { get; set; } = new();
}

public class WorkOrderTestResponse
{
    public int TestId { get; set; }
    public string TestName { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public int Quantity { get; set; }
    public decimal Amount { get; set; }
}

public class WorkOrderResponse
{
    public int WorkOrderId { get; set; }
    public string WoeNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string PatientCode { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<WorkOrderTestResponse> Tests { get; set; } = new();
}
