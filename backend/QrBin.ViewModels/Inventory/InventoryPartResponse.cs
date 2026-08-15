namespace QrBin.ViewModels.Inventory;

public class InventoryPartResponse
{
    public int Id { get; set; }
    public string PartName { get; set; } = string.Empty;
    public string? PartNumber { get; set; }
    public string? BinCode { get; set; }
    public string? QrToken { get; set; }
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }
    public string Status { get; set; } = string.Empty;
}
