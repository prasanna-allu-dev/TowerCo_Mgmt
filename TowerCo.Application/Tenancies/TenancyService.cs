using TowerCo.Application.Abstractions;
using TowerCo.Application.Sites;
using TowerCo.Domain.Entities;
using TowerCo.Domain.Exceptions;
using TowerCo.Domain.ValueObjects;

namespace TowerCo.Application.Tenancies;

public interface ITenancyService
{
    Task<TenancyDto> AddAsync(int towerId, CreateTenancyRequest request, CancellationToken cancellationToken);
    Task<TenancyDto> EndAsync(int towerId, int tenancyId, EndTenancyRequest request, CancellationToken cancellationToken);
    Task<TenancyDto> RenewAsync(int towerId, int tenancyId, RenewTenancyRequest request, CancellationToken cancellationToken);
    Task<TenancyDto> ReviewRentAsync(int towerId, int tenancyId, RentReviewRequest request, CancellationToken cancellationToken);
}

/// <summary>UC2 and the tenancy lifecycle. Every rule (capacity over time, duplicates, terms) lives on the Tower aggregate.</summary>
internal sealed class TenancyService(ITowerRepository towers, IOperatorRepository operators, IUnitOfWork unitOfWork)
    : ITenancyService
{
    public async Task<TenancyDto> AddAsync(int towerId, CreateTenancyRequest request, CancellationToken cancellationToken)
    {
        var tower = await LoadTower(towerId, cancellationToken);
        var mobileOperator = await operators.FindAsync(request.OperatorId, cancellationToken)
            ?? throw new NotFoundException(nameof(Operator), request.OperatorId);

        var rent = new Money(request.MonthlyRent, request.Currency);
        var tenancy = tower.AddTenancy(mobileOperator, request.StartDate, request.EndDate, rent);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return SiteMapper.ToTenancy(tenancy);
    }

    public Task<TenancyDto> EndAsync(int towerId, int tenancyId, EndTenancyRequest request, CancellationToken cancellationToken) =>
        ChangeAsync(towerId, tenancyId, (tower, tenancy) => tower.EndTenancy(tenancy, request.EndDate), cancellationToken);

    public Task<TenancyDto> RenewAsync(int towerId, int tenancyId, RenewTenancyRequest request, CancellationToken cancellationToken) =>
        ChangeAsync(towerId, tenancyId, (tower, tenancy) => tower.RenewTenancy(tenancy, request.EndDate), cancellationToken);

    public Task<TenancyDto> ReviewRentAsync(int towerId, int tenancyId, RentReviewRequest request, CancellationToken cancellationToken) =>
        ChangeAsync(towerId, tenancyId,
            (tower, tenancy) => tower.ReviewRent(tenancy, new Money(request.MonthlyRent, tenancy.MonthlyRent.Currency)), cancellationToken);

    private async Task<TenancyDto> ChangeAsync(int towerId, int tenancyId, Action<Tower, Tenancy> change, CancellationToken cancellationToken)
    {
        var tower = await LoadTower(towerId, cancellationToken);
        var tenancy = tower.Tenancies.FirstOrDefault(t => t.Id == tenancyId)
            ?? throw new NotFoundException(nameof(Tenancy), tenancyId);
        change(tower, tenancy);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return SiteMapper.ToTenancy(tenancy);
    }

    private async Task<Tower> LoadTower(int towerId, CancellationToken cancellationToken) =>
        await towers.GetForUpdateAsync(towerId, cancellationToken) ?? throw new NotFoundException(nameof(Tower), towerId);
}
