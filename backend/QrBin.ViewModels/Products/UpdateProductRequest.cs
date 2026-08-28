namespace QrBin.ViewModels.Products;

/// <summary>Deliberately has no stock field — current stock only ever changes via Billing or Add Stock.</summary>
public class UpdateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string Unit { get; set; } = "pcs";
    public decimal SellingPrice { get; set; }
}
