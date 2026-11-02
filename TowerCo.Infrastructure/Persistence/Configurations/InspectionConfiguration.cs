using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TowerCo.Domain.Monitoring;

namespace TowerCo.Infrastructure.Persistence.Configurations;

internal sealed class InspectionConfiguration : IEntityTypeConfiguration<Inspection>
{
    public void Configure(EntityTypeBuilder<Inspection> builder)
    {
        builder.Property(i => i.Inspector).HasMaxLength(ColumnLengths.Name).IsRequired();
        builder.Property(i => i.Notes).HasMaxLength(ColumnLengths.Description);
        builder.Property(i => i.Outcome).HasConversion<string>().HasMaxLength(ColumnLengths.EnumText);
        builder.HasOne(i => i.Site).WithMany().HasForeignKey(i => i.SiteId).OnDelete(DeleteBehavior.Cascade);
    }
}
