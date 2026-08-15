using Microsoft.EntityFrameworkCore;
using QrBin.Data.Context;

namespace QrBin.Data.Repositories;

public interface IDashboardRepository
{
    Task<DashboardSummary> GetSummaryAsync(int organizationId, CancellationToken cancellationToken = default);
}

public class DashboardSummary
{
    public int TotalParts { get; set; }
    public int LowStockCount { get; set; }
    public int OutOfStockCount { get; set; }
    public List<LowStockPartSummary> LowStockParts { get; set; } = [];
}

public class LowStockPartSummary
{
    public int Id { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }
    public string? BinCode { get; set; }
}

public class DashboardRepository : IDashboardRepository
{
    private readonly QrBinDbContext _context;

    public DashboardRepository(QrBinDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummary> GetSummaryAsync(int organizationId, CancellationToken cancellationToken = default)
    {
        var totalParts = await _context.Parts
            .CountAsync(p => p.OrganizationId == organizationId, cancellationToken);

        var outOfStockCount = await _context.Parts
            .CountAsync(p => p.OrganizationId == organizationId && p.CurrentStock <= 0, cancellationToken);

        // "Low Stock" excludes Out of Stock — the two counts are mutually exclusive buckets.
        var lowStockParts = await _context.Parts
            .Where(p => p.OrganizationId == organizationId && p.CurrentStock > 0 && p.CurrentStock <= p.MinimumStock)
            .OrderBy(p => p.CurrentStock)
            .Take(10)
            .Select(p => new LowStockPartSummary
            {
                Id = p.Id,
                PartName = p.PartName,
                CurrentStock = p.CurrentStock,
                MinimumStock = p.MinimumStock,
                BinCode = p.Bin != null ? p.Bin.Code : null
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var lowStockCount = await _context.Parts
            .CountAsync(p => p.OrganizationId == organizationId && p.CurrentStock > 0 && p.CurrentStock <= p.MinimumStock, cancellationToken);

        return new DashboardSummary
        {
            TotalParts = totalParts,
            LowStockCount = lowStockCount,
            OutOfStockCount = outOfStockCount,
            LowStockParts = lowStockParts
        };
    }
}
