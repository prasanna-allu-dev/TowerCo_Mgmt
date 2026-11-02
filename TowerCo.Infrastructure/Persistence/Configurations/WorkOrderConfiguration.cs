using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TowerCo.Domain.Entities;

namespace TowerCo.Infrastructure.Persistence.Configurations;

internal sealed class WorkOrderConfiguration : IEntityTypeConfiguration<WorkOrder>
{
    public void Configure(EntityTypeBuilder<WorkOrder> builder)
    {
        builder.Property(w => w.Title).HasMaxLength(ColumnLengths.Name).IsRequired();
        builder.Property(w => w.Description).HasMaxLength(ColumnLengths.Description);
        builder.Property(w => w.Assignee).HasMaxLength(ColumnLengths.Name);
        builder.Property(w => w.Category).HasConversion<string>().HasMaxLength(ColumnLengths.EnumText);
        builder.Property(w => w.Priority).HasConversion<string>().HasMaxLength(ColumnLengths.EnumText);
        builder.Property(w => w.Status).HasConversion<string>().HasMaxLength(ColumnLengths.EnumText);
        builder.HasOne(w => w.Site).WithMany()
            .HasForeignKey(w => w.SiteId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(w => new { w.SiteId, w.Status });
    }
}
