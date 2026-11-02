using Microsoft.EntityFrameworkCore;
using TowerCo.Application.Abstractions;
using TowerCo.Domain.Entities;

namespace TowerCo.Infrastructure.Persistence.Repositories;

public sealed class WorkOrderRepository(TowerCoDbContext db) : IWorkOrderRepository
{
    public async Task<IReadOnlyList<WorkOrder>> ListAsync(CancellationToken cancellationToken) =>
        await db.WorkOrders.Include(w => w.Site).AsNoTracking().ToListAsync(cancellationToken);

    public Task<WorkOrder?> GetAsync(int id, CancellationToken cancellationToken) =>
        db.WorkOrders.Include(w => w.Site).FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

    public void Add(WorkOrder workOrder) => db.WorkOrders.Add(workOrder);
}
