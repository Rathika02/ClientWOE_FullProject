using System.ComponentModel.DataAnnotations;

namespace ClientWOE.API.Models;

public class TestMaster
{
    [Key]
    public int TestId { get; set; }

    [Required, MaxLength(30)]
    public string TestCode { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string TestName { get; set; } = string.Empty;

    [Range(0.01, 1000000)]
    public decimal Rate { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<WorkOrderTestDetail> WorkOrderTestDetails { get; set; } = new List<WorkOrderTestDetail>();
}
