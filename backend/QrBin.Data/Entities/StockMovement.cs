namespace QrBin.Data.Entities;

public enum StockMovementReason
{
    OpeningStock,
    Sale,
    StockReceived,

    /// <summary>Customer returned units of a product after the bill was paid — stock goes back up.</summary>
    Return,

    /// <summary>The replacement side of a customer exchange — stock goes down for the new product given out.</summary>
    Exchange
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

    /// <summary>Set when Reason is Sale, Return, or Exchange.</summary>
    public int? BillId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Organization Organization { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public Bill? Bill { get; set; }
}
