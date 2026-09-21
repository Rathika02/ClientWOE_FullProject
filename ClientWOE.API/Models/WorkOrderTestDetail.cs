using System.ComponentModel.DataAnnotations;

namespace ClientWOE.API.Models;

public class WorkOrderTestDetail
{
    public int WorkOrderTestDetailId { get; set; }

    public int WorkOrderId { get; set; }
    public WorkOrder? WorkOrder { get; set; }

    public int TestId { get; set; }
    public TestMaster? TestMaster { get; set; }

    [Range(1, 1000)]
    public int Quantity { get; set; }

    [Range(0.01, 1000000)]
    public decimal Rate { get; set; }

    [Range(0.01, 10000000)]
    public decimal Amount { get; set; }
}
