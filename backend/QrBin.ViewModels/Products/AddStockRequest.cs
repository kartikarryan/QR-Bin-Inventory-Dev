namespace QrBin.ViewModels.Products;

public class AddStockRequest
{
    /// <summary>Quantity received — always positive. Server computes CurrentStock + Quantity, never accepts a new total directly.</summary>
    public int Quantity { get; set; }
}
