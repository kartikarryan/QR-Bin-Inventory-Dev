using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QrBin.Data.Entities;

namespace QrBin.Data.Context.Configuration;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Product");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Code).HasMaxLength(100);
        builder.Property(p => p.Unit).HasMaxLength(20).IsRequired();
        builder.Property(p => p.SellingPrice).HasColumnType("numeric(12,2)");
        builder.Property(p => p.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(p => p.UpdatedAt).HasDefaultValueSql("now()");

        builder.HasIndex(p => new { p.OrganizationId, p.Name });
        builder.HasIndex(p => new { p.OrganizationId, p.Code }).IsUnique().HasFilter("\"Code\" IS NOT NULL");

        builder.HasOne(p => p.Organization)
            .WithMany(o => o.Products)
            .HasForeignKey(p => p.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
