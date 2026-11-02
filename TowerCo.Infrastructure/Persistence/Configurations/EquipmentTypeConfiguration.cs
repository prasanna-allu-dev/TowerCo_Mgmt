using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TowerCo.Domain.Entities;

namespace TowerCo.Infrastructure.Persistence.Configurations;

internal sealed class EquipmentTypeConfiguration : IEntityTypeConfiguration<EquipmentType>
{
    public void Configure(EntityTypeBuilder<EquipmentType> builder)
    {
        builder.HasIndex(e => e.Name).IsUnique();
        builder.Property(e => e.Name).HasMaxLength(ColumnLengths.Name).IsRequired();
        builder.Property(e => e.Category).HasConversion<string>().HasMaxLength(ColumnLengths.EnumText);
    }
}
