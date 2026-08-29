namespace QrBin.ViewModels.Products;

/// <summary>Cross-product movement log row — unlike StockMovementResponse, the product isn't
/// already implied by the URL, so this carries the product's identity too.</summary>
public class StockMovementLogResponse
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;

    /// <summary>"Opening Stock" / "Sold" / "Stock Received" — plain language, not the raw enum name.</summary>
    public string Reason { get; set; } = string.Empty;

    public int QuantityDelta { get; set; }
    public int NewStock { get; set; }
    public int? BillId { get; set; }
    public DateTime CreatedAt { get; set; }
}
