using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TowerCo.Domain.Entities;

namespace TowerCo.Infrastructure.Persistence.Configurations;

internal sealed class OperatorConfiguration : IEntityTypeConfiguration<Operator>
{
    public void Configure(EntityTypeBuilder<Operator> builder)
    {
        builder.HasIndex(o => o.Name).IsUnique();
        builder.Property(o => o.Name).HasMaxLength(ColumnLengths.Name).IsRequired();
        builder.Property(o => o.CountryCode).HasMaxLength(ColumnLengths.CountryCode).IsRequired();
    }
}
