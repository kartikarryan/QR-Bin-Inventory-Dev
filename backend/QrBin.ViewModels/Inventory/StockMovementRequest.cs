namespace QrBin.ViewModels.Inventory;

public class StockMovementRequest
{
    /// <summary>Positive to add stock, negative to remove it. Never zero.</summary>
    public int QuantityDelta { get; set; }
}
