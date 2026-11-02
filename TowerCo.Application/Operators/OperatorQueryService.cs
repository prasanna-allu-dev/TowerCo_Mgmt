using TowerCo.Application.Abstractions;

namespace TowerCo.Application.Operators;

public sealed record OperatorDto(int Id, string Name, string CountryCode);

public interface IOperatorQueryService
{
    Task<IReadOnlyList<OperatorDto>> ListAsync(CancellationToken cancellationToken);
}

internal sealed class OperatorQueryService(IOperatorRepository operators) : IOperatorQueryService
{
    public async Task<IReadOnlyList<OperatorDto>> ListAsync(CancellationToken cancellationToken)
    {
        var all = await operators.ListAsync(cancellationToken);
        return all.Select(o => new OperatorDto(o.Id, o.Name, o.CountryCode)).ToList();
    }
}
