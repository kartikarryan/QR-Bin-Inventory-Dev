using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QrBin.Data.Entities;

namespace QrBin.Data.Context.Configuration;

public class StockTransactionConfiguration : IEntityTypeConfiguration<StockTransaction>
{
    public void Configure(EntityTypeBuilder<StockTransaction> builder)
    {
        builder.ToTable("StockTransaction");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.CreatedAt).HasDefaultValueSql("now()");

        builder.HasIndex(t => t.CreatedAt);

        builder.HasOne(t => t.Organization)
            .WithMany()
            .HasForeignKey(t => t.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Restrict, not Cascade — this is an audit trail; a Part/Bin should be archived, not deleted, once it has history.
        builder.HasOne(t => t.Part)
            .WithMany(p => p.StockTransactions)
            .HasForeignKey(t => t.PartId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Bin)
            .WithMany(b => b.StockTransactions)
            .HasForeignKey(t => t.BinId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
