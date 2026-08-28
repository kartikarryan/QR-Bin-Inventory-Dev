namespace QrBin.Data.Entities;

public class BillItem
{
    public int Id { get; set; }
    public int BillId { get; set; }
    public int ProductId { get; set; }

    /// <summary>Snapshotted at sale time so a later product rename/repricing doesn't rewrite this bill's history.</summary>
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }

    public Bill Bill { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
