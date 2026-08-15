using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QrBin.Data.Entities;

namespace QrBin.Data.Context.Configuration;

public class ManagerUserConfiguration : IEntityTypeConfiguration<ManagerUser>
{
    public void Configure(EntityTypeBuilder<ManagerUser> builder)
    {
        builder.ToTable("Manager");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Email).HasMaxLength(256).IsRequired();
        builder.Property(m => m.PasswordHash).IsRequired();
        builder.Property(m => m.FullName).HasMaxLength(200).IsRequired();
        builder.Property(m => m.CreatedAt).HasDefaultValueSql("now()");

        builder.HasIndex(m => m.Email).IsUnique();

        builder.HasOne(m => m.Organization)
            .WithMany(o => o.Managers)
            .HasForeignKey(m => m.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
