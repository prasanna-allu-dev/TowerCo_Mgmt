using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TowerCo.Domain.Entities;

namespace TowerCo.Infrastructure.Persistence.Configurations;

internal sealed class TenancyConfiguration : IEntityTypeConfiguration<Tenancy>
{
    public void Configure(EntityTypeBuilder<Tenancy> builder)
    {
        builder.HasOne(t => t.Operator).WithMany()
            .HasForeignKey(t => t.OperatorId).OnDelete(DeleteBehavior.Restrict);
        builder.ComplexProperty(t => t.MonthlyRent, rent =>
        {
            rent.Property(r => r.Amount).HasPrecision(ColumnLengths.MoneyPrecision, ColumnLengths.MoneyScale);
            rent.Property(r => r.Currency).HasMaxLength(ColumnLengths.CurrencyCode).IsRequired();
        });
    }
}
