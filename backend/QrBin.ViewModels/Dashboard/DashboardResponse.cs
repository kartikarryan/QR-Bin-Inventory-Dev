namespace QrBin.ViewModels.Dashboard;

public class DashboardResponse
{
    public int TotalParts { get; set; }
    public int LowStockCount { get; set; }
    public int OutOfStockCount { get; set; }
    public List<LowStockPartResponse> LowStockParts { get; set; } = [];
}

public class LowStockPartResponse
{
    public int Id { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }
    public string? BinCode { get; set; }
}
