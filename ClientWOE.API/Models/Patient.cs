using System.ComponentModel.DataAnnotations;

namespace ClientWOE.API.Models;

public class Patient
{
    public int PatientId { get; set; }

    [Required, MaxLength(30)]
    public string PatientCode { get; set; } = string.Empty;

    public int ClientId { get; set; }
    public Client? Client { get; set; }

    [Required, MaxLength(100)]
    public string PatientName { get; set; } = string.Empty;

    [Range(0, 120)]
    public int Age { get; set; }

    public Gender Gender { get; set; }

    [Required, MaxLength(15)]
    public string MobileNumber { get; set; } = string.Empty;

    public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
}
