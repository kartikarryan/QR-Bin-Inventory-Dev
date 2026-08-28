namespace QrBin.ViewModels.Products;

public class StockMovementResponse
{
    public int Id { get; set; }

    /// <summary>"Opening Stock" / "Sold" / "Stock Received" — plain language, not the raw enum name.</summary>
    public string Reason { get; set; } = string.Empty;

    public int QuantityDelta { get; set; }
    public int NewStock { get; set; }
    public int? BillId { get; set; }
    public DateTime CreatedAt { get; set; }
}
