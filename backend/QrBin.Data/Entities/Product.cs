namespace QrBin.Data.Entities;

public class Product
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? HsnCode { get; set; }
    public string Unit { get; set; } = "pcs";
    public decimal SellingPrice { get; set; }
    public decimal GstRate { get; set; } = 18;
    public bool IsActive { get; set; } = true;

    /// <summary>Never set directly outside CreateAsync (opening stock) — every other change goes through a StockMovement.</summary>
    public int CurrentStock { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Organization Organization { get; set; } = null!;
    public List<StockMovement> StockMovements { get; set; } = [];
    public List<BillItem> BillItems { get; set; } = [];
}
