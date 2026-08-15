using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QrBin.Data.Entities;

namespace QrBin.Data.Context.Configuration;

public class PartConfiguration : IEntityTypeConfiguration<Part>
{
    public void Configure(EntityTypeBuilder<Part> builder)
    {
        builder.ToTable("Part");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.PartName).HasMaxLength(200).IsRequired();
        builder.Property(p => p.PartNumber).HasMaxLength(100);
        builder.Property(p => p.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(p => p.UpdatedAt).HasDefaultValueSql("now()");

        builder.HasIndex(p => new { p.OrganizationId, p.PartNumber }).IsUnique();

        // One active part per bin — nulls (unassigned bins) are excluded from the uniqueness check.
        builder.HasIndex(p => p.BinId).IsUnique().HasFilter("\"BinId\" IS NOT NULL");

        builder.HasOne(p => p.Organization)
            .WithMany(o => o.Parts)
            .HasForeignKey(p => p.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Bin)
            .WithOne(b => b.Part)
            .HasForeignKey<Part>(p => p.BinId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
