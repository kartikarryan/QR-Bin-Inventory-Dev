namespace QrBin.Api.Common.Utility;

public static class PartStatusCalculator
{
    public static string GetStatus(int currentStock, int minimumStock)
    {
        if (currentStock <= 0) return "Out of Stock";
        if (currentStock <= minimumStock) return "Low Stock";
        return "In Stock";
    }
}
