namespace QrBin.ViewModels.Products;

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string Unit { get; set; } = "pcs";
    public decimal SellingPrice { get; set; }

    /// <summary>Only used at creation time — never editable afterward. See UpdateProductRequest.</summary>
    public int OpeningStock { get; set; }
}
