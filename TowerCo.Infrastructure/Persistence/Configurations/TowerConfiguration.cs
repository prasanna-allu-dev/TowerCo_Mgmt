using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TowerCo.Domain.Entities;

namespace TowerCo.Infrastructure.Persistence.Configurations;

internal sealed class TowerConfiguration : IEntityTypeConfiguration<Tower>
{
    public void Configure(EntityTypeBuilder<Tower> builder)
    {
        builder.Property(t => t.TowerType).HasConversion<string>().HasMaxLength(ColumnLengths.EnumText);
        builder.Property(t => t.HeightMetres).HasPrecision(ColumnLengths.MetresPrecision, ColumnLengths.MetresScale);
        builder.HasMany(t => t.Tenancies).WithOne(x => x.Tower)
            .HasForeignKey(x => x.TowerId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(t => t.Equipment).WithOne(x => x.Tower)
            .HasForeignKey(x => x.TowerId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(t => t.Tenancies).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(t => t.Equipment).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
