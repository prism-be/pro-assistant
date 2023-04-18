namespace Prism.Infrastructure.Providers;

public static class FilterOperator
{
    public const string Equal = "eq";
    public const string NotEqual = "ne";
    public const string GreaterThan = "gt";
    public const string GreaterThanOrEqual = "gte";
    public const string LessThan = "lt";
    public const string LessThanOrEqual = "lte";
    public const string Regex = "regex";
}

public record Filter(string Field, object Value, string Operator = FilterOperator.Equal);