using System.ComponentModel.DataAnnotations;

namespace ClientWOE.API.Models;

public class Client
{
    public int ClientId { get; set; }

    [Required, MaxLength(30)]
    public string ClientCode { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string ClientName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<Patient> Patients { get; set; } = new List<Patient>();
    public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
}
