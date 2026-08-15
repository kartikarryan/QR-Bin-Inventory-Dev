using Microsoft.EntityFrameworkCore;
using QrBin.Data.Context;

namespace QrBin.Data.Repositories;

public interface IInventoryRepository
{
    Task<List<InventoryPartSummary>> GetPartsAsync(int organizationId, CancellationToken cancellationToken = default);
}

public class InventoryPartSummary
{
    public int Id { get; set; }
    public string PartName { get; set; } = string.Empty;
    public string? PartNumber { get; set; }
    public string? BinCode { get; set; }
    public string? QrToken { get; set; }
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }
}

public class InventoryRepository : IInventoryRepository
{
    private readonly QrBinDbContext _context;

    public InventoryRepository(QrBinDbContext context)
    {
        _context = context;
    }

    public async Task<List<InventoryPartSummary>> GetPartsAsync(int organizationId, CancellationToken cancellationToken = default)
    {
        return await _context.Parts
            .Where(p => p.OrganizationId == organizationId)
            .OrderBy(p => p.PartName)
            .Select(p => new InventoryPartSummary
            {
                Id = p.Id,
                PartName = p.PartName,
                PartNumber = p.PartNumber,
                BinCode = p.Bin != null ? p.Bin.Code : null,
                QrToken = p.Bin != null ? p.Bin.QrToken : null,
                CurrentStock = p.CurrentStock,
                MinimumStock = p.MinimumStock
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
