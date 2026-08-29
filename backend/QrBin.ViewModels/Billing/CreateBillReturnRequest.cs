namespace QrBin.ViewModels.Billing;

public class CreateBillReturnRequest
{
    public int BillItemId { get; set; }
    public int ReturnedQuantity { get; set; }

    /// <summary>Set together with <see cref="ReplacementQuantity"/> when this is an exchange, not a plain return.</summary>
    public int? ReplacementProductId { get; set; }
    public int? ReplacementQuantity { get; set; }

    public string? Notes { get; set; }
}
