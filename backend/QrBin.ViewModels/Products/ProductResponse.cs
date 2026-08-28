namespace QrBin.ViewModels.Products;

public class ProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal SellingPrice { get; set; }
    public int CurrentStock { get; set; }
}
