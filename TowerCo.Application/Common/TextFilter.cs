namespace TowerCo.Application.Common;

/// <summary>Case-insensitive helpers shared by the list filters.</summary>
public static class TextFilter
{
    /// <summary>True when search is blank or any value contains it.</summary>
    public static bool Matches(string? search, params string[] values) =>
        string.IsNullOrWhiteSpace(search)
        || values.Any(value => value.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase));

    /// <summary>True when filter is blank or equals the value ignoring case.</summary>
    public static bool EqualsCode(string? filter, string value) =>
        string.IsNullOrWhiteSpace(filter) || value.Equals(filter.Trim(), StringComparison.OrdinalIgnoreCase);
}
