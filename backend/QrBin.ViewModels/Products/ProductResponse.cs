namespace QrBin.ViewModels.Products;

public class ProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? HsnCode { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal SellingPrice { get; set; }
    public decimal GstRate { get; set; }
    public int CurrentStock { get; set; }
    public bool IsActive { get; set; }
}
