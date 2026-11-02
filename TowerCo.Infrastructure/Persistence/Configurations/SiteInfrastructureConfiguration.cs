using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TowerCo.Domain.Entities;

namespace TowerCo.Infrastructure.Persistence.Configurations;

internal sealed class SiteInfrastructureConfiguration : IEntityTypeConfiguration<SiteInfrastructure>
{
    public void Configure(EntityTypeBuilder<SiteInfrastructure> builder)
    {
        builder.HasIndex(i => i.SerialNumber).IsUnique();
        builder.Property(i => i.SerialNumber).HasMaxLength(ColumnLengths.Code).IsRequired();
        builder.Property(i => i.Kind).HasConversion<string>().HasMaxLength(ColumnLengths.EnumText);
    }
}
