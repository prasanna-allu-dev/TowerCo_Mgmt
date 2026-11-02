using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TowerCo.Domain.Entities;

namespace TowerCo.Infrastructure.Persistence.Configurations;

internal sealed class SiteConfiguration : IEntityTypeConfiguration<Site>
{
    public void Configure(EntityTypeBuilder<Site> builder)
    {
        builder.HasIndex(s => s.SiteCode).IsUnique();
        builder.Property(s => s.SiteCode).HasMaxLength(ColumnLengths.Code).IsRequired();
        builder.Property(s => s.Name).HasMaxLength(ColumnLengths.Name).IsRequired();
        builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(ColumnLengths.EnumText);
        builder.ComplexProperty(s => s.Location, location =>
        {
            location.Property(l => l.CountryCode).HasMaxLength(ColumnLengths.CountryCode).IsRequired();
            location.Property(l => l.City).HasMaxLength(ColumnLengths.Name).IsRequired();
        });
        builder.HasOne(s => s.Tower).WithOne(t => t.Site)
            .HasForeignKey<Tower>(t => t.SiteId).OnDelete(DeleteBehavior.Cascade);
    }
}
