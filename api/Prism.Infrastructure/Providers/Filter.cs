namespace Prism.Infrastructure.Providers;

public static class FilterOperator
{
    public const string Equal = "eq"; 
}

public record Filter(string Field, object Value, string Operator = FilterOperator.Equal);