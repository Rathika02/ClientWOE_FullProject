using System.ComponentModel.DataAnnotations;

namespace ClientWOE.API.Models;

public class WorkOrder
{
    public int WorkOrderId { get; set; }

    [Required, MaxLength(30)]
    public string WoeNumber { get; set; } = string.Empty;

    public int ClientId { get; set; }
    public Client? Client { get; set; }

    public int PatientId { get; set; }
    public Patient? Patient { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.Now;

    [Range(0, 10000000)]
    public decimal TotalAmount { get; set; }

    [Required, MaxLength(30)]
    public string Status { get; set; } = "Saved";

    public ICollection<WorkOrderTestDetail> TestDetails { get; set; } = new List<WorkOrderTestDetail>();
}
