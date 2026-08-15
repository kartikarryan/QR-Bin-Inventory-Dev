namespace QrBin.Data.Entities;

/// <summary>Append-only audit record — one row per stock change. This is the Activity feed.</summary>
public class StockTransaction
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public int PartId { get; set; }
    public int BinId { get; set; }

    public int QuantityDelta { get; set; }
    public int PreviousStock { get; set; }
    public int NewStock { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Organization Organization { get; set; } = null!;
    public Part Part { get; set; } = null!;
    public Bin Bin { get; set; } = null!;
}
