using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QrBin.Data.Entities;

namespace QrBin.Data.Context.Configuration;

public class BillItemConfiguration : IEntityTypeConfiguration<BillItem>
{
    public void Configure(EntityTypeBuilder<BillItem> builder)
    {
        builder.ToTable("BillItem");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.ProductName).HasMaxLength(200).IsRequired();
        builder.Property(i => i.UnitPrice).HasColumnType("numeric(12,2)");
        builder.Property(i => i.LineTotal).HasColumnType("numeric(12,2)");

        builder.HasOne(i => i.Bill)
            .WithMany(b => b.Items)
            .HasForeignKey(i => i.BillId)
            .OnDelete(DeleteBehavior.Cascade);

        // Restrict, not Cascade — a bill's line items must survive even if the product is later removed.
        builder.HasOne(i => i.Product)
            .WithMany(p => p.BillItems)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
