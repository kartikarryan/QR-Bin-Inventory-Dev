using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QrBin.Data.Entities;

namespace QrBin.Data.Context.Configuration;

public class BillReturnConfiguration : IEntityTypeConfiguration<BillReturn>
{
    public void Configure(EntityTypeBuilder<BillReturn> builder)
    {
        builder.ToTable("BillReturn");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Notes).HasMaxLength(500);
        builder.Property(r => r.CreatedAt).HasDefaultValueSql("now()");

        builder.HasIndex(r => r.BillId);

        builder.HasOne(r => r.Organization)
            .WithMany()
            .HasForeignKey(r => r.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Bill)
            .WithMany(b => b.Returns)
            .HasForeignKey(r => r.BillId)
            .OnDelete(DeleteBehavior.Cascade);

        // Restrict, not Cascade — the original line item must survive as the audit target.
        builder.HasOne(r => r.BillItem)
            .WithMany()
            .HasForeignKey(r => r.BillItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.ReplacementProduct)
            .WithMany()
            .HasForeignKey(r => r.ReplacementProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
