namespace Prism.Infrastructure.Providers;

public static class FilterOperator
{
    public const string Equal = "eq"; 
}

public record Filter(string Property, object Value, string Op = FilterOperator.Equal);