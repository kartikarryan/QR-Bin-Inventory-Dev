namespace QrBin.Data.Entities;

public class Bill
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Organization Organization { get; set; } = null!;
    public List<BillItem> Items { get; set; } = [];
}
