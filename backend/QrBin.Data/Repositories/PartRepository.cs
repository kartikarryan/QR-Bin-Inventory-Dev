using Microsoft.EntityFrameworkCore;
using QrBin.Data.Context;
using QrBin.Data.Entities;

namespace QrBin.Data.Repositories;

public interface IPartRepository
{
    Task<Part?> GetByIdAsync(int organizationId, int partId, CancellationToken cancellationToken = default);
    Task<Part?> GetTrackedByIdAsync(int organizationId, int partId, CancellationToken cancellationToken = default);
    Task<bool> PartNumberExistsAsync(int organizationId, string partNumber, int? excludePartId, CancellationToken cancellationToken = default);
    Task<Bin?> FindBinByCodeAsync(int organizationId, string code, CancellationToken cancellationToken = default);
    Task<Bin?> GetBinByQrTokenAsync(string qrToken, CancellationToken cancellationToken = default);
    Task<Bin?> GetTrackedBinByQrTokenAsync(string qrToken, CancellationToken cancellationToken = default);
    Bin PrepareNewBin(int organizationId, string code);
    Task<int?> GetAssignedPartIdAsync(int binId, int? excludePartId, CancellationToken cancellationToken = default);
    Task AddAsync(Part part, CancellationToken cancellationToken = default);
    Task AddStockTransactionAsync(StockTransaction transaction, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class PartRepository : IPartRepository
{
    private readonly QrBinDbContext _context;

    public PartRepository(QrBinDbContext context)
    {
        _context = context;
    }

    public async Task<Part?> GetByIdAsync(int organizationId, int partId, CancellationToken cancellationToken = default)
    {
        return await _context.Parts
            .Include(p => p.Bin)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.OrganizationId == organizationId && p.Id == partId, cancellationToken);
    }

    public async Task<Part?> GetTrackedByIdAsync(int organizationId, int partId, CancellationToken cancellationToken = default)
    {
        return await _context.Parts
            .Include(p => p.Bin)
            .FirstOrDefaultAsync(p => p.OrganizationId == organizationId && p.Id == partId, cancellationToken);
    }

    public async Task<bool> PartNumberExistsAsync(int organizationId, string partNumber, int? excludePartId, CancellationToken cancellationToken = default)
    {
        return await _context.Parts.AnyAsync(
            p => p.OrganizationId == organizationId
                 && p.PartNumber == partNumber
                 && (excludePartId == null || p.Id != excludePartId),
            cancellationToken);
    }

    public async Task<Bin?> FindBinByCodeAsync(int organizationId, string code, CancellationToken cancellationToken = default)
    {
        return await _context.Bins
            .FirstOrDefaultAsync(b => b.OrganizationId == organizationId && b.Code == code, cancellationToken);
    }

    public async Task<Bin?> GetBinByQrTokenAsync(string qrToken, CancellationToken cancellationToken = default)
    {
        return await _context.Bins
            .Include(b => b.Part)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.QrToken == qrToken, cancellationToken);
    }

    public async Task<Bin?> GetTrackedBinByQrTokenAsync(string qrToken, CancellationToken cancellationToken = default)
    {
        return await _context.Bins
            .Include(b => b.Part)
            .FirstOrDefaultAsync(b => b.QrToken == qrToken, cancellationToken);
    }

    public Bin PrepareNewBin(int organizationId, string code)
    {
        var bin = new Bin { OrganizationId = organizationId, Code = code };
        _context.Bins.Add(bin);
        return bin;
    }

    public async Task<int?> GetAssignedPartIdAsync(int binId, int? excludePartId, CancellationToken cancellationToken = default)
    {
        return await _context.Parts
            .Where(p => p.BinId == binId && (excludePartId == null || p.Id != excludePartId))
            .Select(p => (int?)p.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(Part part, CancellationToken cancellationToken = default)
    {
        await _context.Parts.AddAsync(part, cancellationToken);
    }

    public async Task AddStockTransactionAsync(StockTransaction transaction, CancellationToken cancellationToken = default)
    {
        await _context.StockTransactions.AddAsync(transaction, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
