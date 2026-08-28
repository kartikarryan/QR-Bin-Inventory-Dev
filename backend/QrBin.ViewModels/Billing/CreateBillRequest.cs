namespace QrBin.ViewModels.Billing;

public class CreateBillRequest
{
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public List<CreateBillItemRequest> Items { get; set; } = [];
}

public class CreateBillItemRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
