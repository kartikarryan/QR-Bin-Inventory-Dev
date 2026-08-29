using Microsoft.EntityFrameworkCore;
using QrBin.Data.Context;
using QrBin.Data.Entities;

namespace QrBin.Data.Repositories;

public interface IBillRepository
{
    Task<List<Bill>> GetAllAsync(int organizationId, CancellationToken cancellationToken = default);
    Task<Bill?> GetByIdAsync(int organizationId, int billId, CancellationToken cancellationToken = default);
    Task<Bill?> GetTrackedByIdAsync(int organizationId, int billId, CancellationToken cancellationToken = default);
    Task AddAsync(Bill bill, CancellationToken cancellationToken = default);
    Task AddReturnAsync(BillReturn billReturn, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class BillRepository : IBillRepository
{
    private readonly QrBinDbContext _context;

    public BillRepository(QrBinDbContext context)
    {
        _context = context;
    }

    public async Task<List<Bill>> GetAllAsync(int organizationId, CancellationToken cancellationToken = default)
    {
        return await _context.Bills
            .AsNoTracking()
            .Include(b => b.Items)
            .Where(b => b.OrganizationId == organizationId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Bill?> GetByIdAsync(int organizationId, int billId, CancellationToken cancellationToken = default)
    {
        return await _context.Bills
            .AsNoTracking()
            .Include(b => b.Items)
            .Include(b => b.Returns).ThenInclude(r => r.ReplacementProduct)
            .FirstOrDefaultAsync(b => b.OrganizationId == organizationId && b.Id == billId, cancellationToken);
    }

    public async Task<Bill?> GetTrackedByIdAsync(int organizationId, int billId, CancellationToken cancellationToken = default)
    {
        return await _context.Bills
            .Include(b => b.Items)
            .Include(b => b.Returns)
            .FirstOrDefaultAsync(b => b.OrganizationId == organizationId && b.Id == billId, cancellationToken);
    }

    public async Task AddAsync(Bill bill, CancellationToken cancellationToken = default)
    {
        await _context.Bills.AddAsync(bill, cancellationToken);
    }

    public async Task AddReturnAsync(BillReturn billReturn, CancellationToken cancellationToken = default)
    {
        await _context.BillReturns.AddAsync(billReturn, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
