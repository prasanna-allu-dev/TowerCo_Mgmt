using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TowerCo.Domain.Entities;

namespace TowerCo.Infrastructure.Persistence.Configurations;

internal sealed class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.HasIndex(e => e.SerialNumber).IsUnique();
        builder.Property(e => e.SerialNumber).HasMaxLength(ColumnLengths.Code).IsRequired();
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(ColumnLengths.EnumText);
        builder.Property(e => e.MountHeightMetres).HasPrecision(ColumnLengths.MetresPrecision, ColumnLengths.MetresScale);
        builder.HasOne(e => e.EquipmentType).WithMany()
            .HasForeignKey(e => e.EquipmentTypeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Tenancy).WithMany()
            .HasForeignKey(e => e.TenancyId).IsRequired().OnDelete(DeleteBehavior.Restrict);
    }
}
