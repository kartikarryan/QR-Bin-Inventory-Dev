namespace QrBin.Data.Entities;

public enum StockMovementReason
{
    OpeningStock,
    Sale,
    StockReceived
}

/// <summary>Append-only audit record — one row per stock change. Powers the plain-language Stock History view.</summary>
public class StockMovement
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public int ProductId { get; set; }
    public StockMovementReason Reason { get; set; }

    public int QuantityDelta { get; set; }
    public int PreviousStock { get; set; }
    public int NewStock { get; set; }

    /// <summary>Set only when Reason == Sale.</summary>
    public int? BillId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Organization Organization { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public Bill? Bill { get; set; }
}
