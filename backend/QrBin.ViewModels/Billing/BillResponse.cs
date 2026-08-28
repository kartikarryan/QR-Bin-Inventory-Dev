namespace QrBin.ViewModels.Billing;

public class BillResponse
{
    public int Id { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<BillItemResponse> Items { get; set; } = [];
}

public class BillItemResponse
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}
