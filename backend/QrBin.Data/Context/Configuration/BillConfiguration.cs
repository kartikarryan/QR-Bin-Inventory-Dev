using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QrBin.Data.Entities;

namespace QrBin.Data.Context.Configuration;

public class BillConfiguration : IEntityTypeConfiguration<Bill>
{
    public void Configure(EntityTypeBuilder<Bill> builder)
    {
        builder.ToTable("Bill");

        builder.HasKey(b => b.Id);
        builder.Property(b => b.CustomerName).HasMaxLength(200);
        builder.Property(b => b.CustomerPhone).HasMaxLength(30);
        builder.Property(b => b.Subtotal).HasColumnType("numeric(12,2)");
        builder.Property(b => b.DiscountAmount).HasColumnType("numeric(12,2)");
        builder.Property(b => b.Total).HasColumnType("numeric(12,2)");
        builder.Property(b => b.CreatedAt).HasDefaultValueSql("now()");

        builder.HasIndex(b => b.CreatedAt);

        builder.HasOne(b => b.Organization)
            .WithMany(o => o.Bills)
            .HasForeignKey(b => b.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
