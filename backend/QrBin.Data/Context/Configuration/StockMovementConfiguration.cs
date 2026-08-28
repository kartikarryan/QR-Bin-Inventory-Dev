using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QrBin.Data.Entities;

namespace QrBin.Data.Context.Configuration;

public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("StockMovement");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Reason).HasConversion<string>().HasMaxLength(20);
        builder.Property(m => m.CreatedAt).HasDefaultValueSql("now()");

        builder.HasIndex(m => m.CreatedAt);
        builder.HasIndex(m => new { m.ProductId, m.CreatedAt });

        builder.HasOne(m => m.Organization)
            .WithMany()
            .HasForeignKey(m => m.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Restrict, not Cascade — this is an audit trail; a product should be archived, not deleted, once it has history.
        builder.HasOne(m => m.Product)
            .WithMany(p => p.StockMovements)
            .HasForeignKey(m => m.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Bill)
            .WithMany()
            .HasForeignKey(m => m.BillId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
