using Microsoft.EntityFrameworkCore;
using TowerCo.Domain.Entities;
using TowerCo.Domain.Monitoring;

namespace TowerCo.Infrastructure.Persistence;

public sealed class TowerCoDbContext(DbContextOptions<TowerCoDbContext> options) : DbContext(options)
{
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<Tower> Towers => Set<Tower>();
    public DbSet<Operator> Operators => Set<Operator>();
    public DbSet<Tenancy> Tenancies => Set<Tenancy>();
    public DbSet<EquipmentType> EquipmentTypes => Set<EquipmentType>();
    public DbSet<Equipment> Equipment => Set<Equipment>();
    public DbSet<SiteInfrastructure> SiteInfrastructure => Set<SiteInfrastructure>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<SensorReading> SensorReadings => Set<SensorReading>();
    public DbSet<Inspection> Inspections => Set<Inspection>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TowerCoDbContext).Assembly);
    }
}
