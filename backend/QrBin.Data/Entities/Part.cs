namespace QrBin.Data.Entities;

public class Part
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public int? BinId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public string? PartNumber { get; set; }
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }

    /// <summary>True once the low-stock email has fired for the current dip; reset when stock rises back above minimum.</summary>
    public bool LowStockNotificationSent { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Organization Organization { get; set; } = null!;
    public Bin? Bin { get; set; }
    public List<StockTransaction> StockTransactions { get; set; } = [];
}
