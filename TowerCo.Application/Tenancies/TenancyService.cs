using TowerCo.Application.Abstractions;
using TowerCo.Application.Sites;
using TowerCo.Domain.Entities;
using TowerCo.Domain.Exceptions;
using TowerCo.Domain.ValueObjects;

namespace TowerCo.Application.Tenancies;

public interface ITenancyService
{
    Task<TenancyDto> AddAsync(int towerId, CreateTenancyRequest request, CancellationToken cancellationToken);
}

/// <summary>UC2. Business rules (active site, duplicate operator, capacity) live on the Tower aggregate.</summary>
internal sealed class TenancyService(ITowerRepository towers, IOperatorRepository operators, IUnitOfWork unitOfWork)
    : ITenancyService
{
    public async Task<TenancyDto> AddAsync(int towerId, CreateTenancyRequest request, CancellationToken cancellationToken)
    {
        var tower = await towers.GetForUpdateAsync(towerId, cancellationToken)
            ?? throw new NotFoundException(nameof(Tower), towerId);
        var mobileOperator = await operators.FindAsync(request.OperatorId, cancellationToken)
            ?? throw new NotFoundException(nameof(Operator), request.OperatorId);

        var rent = new Money(request.MonthlyRent, request.Currency);
        var tenancy = tower.AddTenancy(mobileOperator, request.StartDate, null, rent);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return SiteMapper.ToTenancy(tenancy);
    }
}
