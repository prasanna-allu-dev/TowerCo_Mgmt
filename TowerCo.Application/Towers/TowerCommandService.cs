using System.ComponentModel.DataAnnotations;
using TowerCo.Application.Abstractions;
using TowerCo.Application.Common;
using TowerCo.Application.Constants;
using TowerCo.Application.Sites;
using TowerCo.Domain.Entities;
using TowerCo.Domain.Exceptions;

namespace TowerCo.Application.Towers;

/// <summary>Structural upgrade or downgrade of tenant slots.</summary>
public sealed class ChangeCapacityRequest
{
    [Range(ValidationRules.MinTenantsPerTower, ValidationRules.MaxTenantsPerTower)]
    public required int MaxTenants { get; init; }
}

public interface ITowerCommandService
{
    Task<CapacitySummaryDto> ChangeCapacityAsync(int towerId, ChangeCapacityRequest request, CancellationToken cancellationToken);
}

/// <summary>Capacity cannot drop below the tenancies already committed (rule on the Tower aggregate).</summary>
internal sealed class TowerCommandService(ITowerRepository towers, IUnitOfWork unitOfWork, TimeProvider timeProvider)
    : ITowerCommandService
{
    public async Task<CapacitySummaryDto> ChangeCapacityAsync(int towerId, ChangeCapacityRequest request, CancellationToken cancellationToken)
    {
        var tower = await towers.GetForUpdateAsync(towerId, cancellationToken)
            ?? throw new NotFoundException(nameof(Tower), towerId);
        var today = timeProvider.Today();
        tower.ChangeCapacity(request.MaxTenants, today);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return SiteMapper.ToCapacity(tower, today);
    }
}
