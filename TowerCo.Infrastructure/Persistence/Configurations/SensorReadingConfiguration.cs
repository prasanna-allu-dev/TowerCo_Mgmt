using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TowerCo.Domain.Monitoring;

namespace TowerCo.Infrastructure.Persistence.Configurations;

/// <summary>SensorReading is an immutable domain record, so the table key is a shadow property.</summary>
internal sealed class SensorReadingConfiguration : IEntityTypeConfiguration<SensorReading>
{
    private const string ShadowKey = "Id";

    public void Configure(EntityTypeBuilder<SensorReading> builder)
    {
        builder.Property<int>(ShadowKey);
        builder.HasKey(ShadowKey);
        builder.Property(r => r.SiteCode).HasMaxLength(ColumnLengths.Code).IsRequired();
        builder.Property(r => r.SensorType).HasConversion<string>().HasMaxLength(ColumnLengths.EnumText);
        builder.Property(r => r.Value).HasPrecision(ColumnLengths.MoneyPrecision, ColumnLengths.ReadingScale);
        builder.HasIndex(r => new { r.SiteCode, r.RecordedAtUtc });
    }
}
