namespace QrBin.ViewModels.Billing;

/// <summary>Lighter-weight shape for the Bill History list — no line items.</summary>
public class BillSummaryResponse
{
    public int Id { get; set; }
    public string? CustomerName { get; set; }
    public int ItemCount { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
}
