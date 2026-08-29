namespace QrBin.Data.Entities;

/// <summary>
/// A customer bringing an item back after a bill was paid — either a partial/full return
/// (stock goes back up) or an exchange for a different product (original goes back up,
/// replacement goes down). Linked to the original bill rather than editing it, so the bill
/// stays an accurate record of what was actually sold that day.
/// </summary>
public class BillReturn
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public int BillId { get; set; }
    public int BillItemId { get; set; }

    public int ReturnedQuantity { get; set; }

    /// <summary>Set only when this return is an exchange for a different product.</summary>
    public int? ReplacementProductId { get; set; }
    public int? ReplacementQuantity { get; set; }

    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Organization Organization { get; set; } = null!;
    public Bill Bill { get; set; } = null!;
    public BillItem BillItem { get; set; } = null!;
    public Product? ReplacementProduct { get; set; }
}
