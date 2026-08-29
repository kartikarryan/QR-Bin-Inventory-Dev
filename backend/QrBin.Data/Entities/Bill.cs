namespace QrBin.Data.Entities;

public class Bill
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }

    /// <summary>Sum of line totals before the discount is applied.</summary>
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }

    /// <summary>Subtotal minus DiscountAmount — what the customer actually paid.</summary>
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Organization Organization { get; set; } = null!;
    public List<BillItem> Items { get; set; } = [];
    public List<BillReturn> Returns { get; set; } = [];
}
