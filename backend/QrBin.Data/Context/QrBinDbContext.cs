using System.Reflection;
using Microsoft.EntityFrameworkCore;
using QrBin.Data.Entities;

namespace QrBin.Data.Context;

public class QrBinDbContext : DbContext
{
    public QrBinDbContext(DbContextOptions<QrBinDbContext> options) : base(options)
    {
    }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<ManagerUser> Managers => Set<ManagerUser>();
    public DbSet<Bin> Bins => Set<Bin>();
    public DbSet<Part> Parts => Set<Part>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<Bill> Bills => Set<Bill>();
    public DbSet<BillItem> BillItems => Set<BillItem>();
    public DbSet<BillReturn> BillReturns => Set<BillReturn>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
