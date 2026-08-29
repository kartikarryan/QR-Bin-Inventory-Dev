namespace QrBin.ViewModels.Billing;

public class BillResponse
{
    public int Id { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<BillItemResponse> Items { get; set; } = [];
    public List<BillReturnResponse> Returns { get; set; } = [];
}

public class BillItemResponse
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }

    /// <summary>Sum of ReturnedQuantity across this item's returns/exchanges — how many of Quantity have already come back.</summary>
    public int ReturnedQuantity { get; set; }
}

public class BillReturnResponse
{
    public int Id { get; set; }
    public int BillItemId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int ReturnedQuantity { get; set; }
    public int? ReplacementProductId { get; set; }
    public string? ReplacementProductName { get; set; }
    public int? ReplacementQuantity { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
