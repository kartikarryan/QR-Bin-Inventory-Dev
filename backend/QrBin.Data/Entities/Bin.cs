namespace QrBin.Data.Entities;

public class Bin
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Location { get; set; }

    /// <summary>Opaque token embedded in the printed QR — identifies the bin, never the stock level.</summary>
    public string QrToken { get; set; } = Guid.NewGuid().ToString("N");

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Organization Organization { get; set; } = null!;
    public Part? Part { get; set; }
    public List<StockTransaction> StockTransactions { get; set; } = [];
}
