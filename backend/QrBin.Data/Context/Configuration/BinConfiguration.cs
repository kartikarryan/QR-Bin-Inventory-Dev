using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QrBin.Data.Entities;

namespace QrBin.Data.Context.Configuration;

public class BinConfiguration : IEntityTypeConfiguration<Bin>
{
    public void Configure(EntityTypeBuilder<Bin> builder)
    {
        builder.ToTable("Bin");

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Code).HasMaxLength(50).IsRequired();
        builder.Property(b => b.Location).HasMaxLength(300);
        builder.Property(b => b.QrToken).HasMaxLength(64).IsRequired();
        builder.Property(b => b.CreatedAt).HasDefaultValueSql("now()");

        builder.HasIndex(b => new { b.OrganizationId, b.Code }).IsUnique();
        builder.HasIndex(b => b.QrToken).IsUnique();

        builder.HasOne(b => b.Organization)
            .WithMany(o => o.Bins)
            .HasForeignKey(b => b.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
