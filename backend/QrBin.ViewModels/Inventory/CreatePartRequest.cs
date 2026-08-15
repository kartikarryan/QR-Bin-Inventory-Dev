namespace QrBin.ViewModels.Inventory;

public class CreatePartRequest
{
    public string PartName { get; set; } = string.Empty;
    public string? PartNumber { get; set; }
    public string BinCode { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }
}
