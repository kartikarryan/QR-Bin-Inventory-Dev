using Microsoft.EntityFrameworkCore;
using QrBin.Data.Context;
using QrBin.Data.Entities;

namespace QrBin.Data.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(int organizationId, string? search, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(int organizationId, int productId, CancellationToken cancellationToken = default);
    Task<Product?> GetTrackedByIdAsync(int organizationId, int productId, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(int organizationId, string code, int? excludeProductId, CancellationToken cancellationToken = default);
    Task<List<StockMovement>> GetStockHistoryAsync(int organizationId, int productId, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task AddStockMovementAsync(StockMovement movement, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class ProductRepository : IProductRepository
{
    private readonly QrBinDbContext _context;

    public ProductRepository(QrBinDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync(int organizationId, string? search, CancellationToken cancellationToken = default)
    {
        var query = _context.Products.AsNoTracking().Where(p => p.OrganizationId == organizationId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(p => EF.Functions.ILike(p.Name, $"%{term}%") || (p.Code != null && EF.Functions.ILike(p.Code, $"%{term}%")));
        }

        return await query.OrderBy(p => p.Name).ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(int organizationId, int productId, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.OrganizationId == organizationId && p.Id == productId, cancellationToken);
    }

    public async Task<Product?> GetTrackedByIdAsync(int organizationId, int productId, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.OrganizationId == organizationId && p.Id == productId, cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(int organizationId, string code, int? excludeProductId, CancellationToken cancellationToken = default)
    {
        return await _context.Products.AnyAsync(
            p => p.OrganizationId == organizationId
                 && p.Code == code
                 && (excludeProductId == null || p.Id != excludeProductId),
            cancellationToken);
    }

    public async Task<List<StockMovement>> GetStockHistoryAsync(int organizationId, int productId, CancellationToken cancellationToken = default)
    {
        return await _context.StockMovements
            .AsNoTracking()
            .Where(m => m.OrganizationId == organizationId && m.ProductId == productId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
    }

    public async Task AddStockMovementAsync(StockMovement movement, CancellationToken cancellationToken = default)
    {
        await _context.StockMovements.AddAsync(movement, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
