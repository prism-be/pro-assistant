using System.Text.Json.Serialization;

namespace Prism.ProAssistant.Api.Models;

public class OperationResult
{
    [JsonPropertyName("success")]
    required public bool Success { get; set; }

    public static OperationResult? From(bool result)
    {
        return new OperationResult
        {
            Success = result
        };
    }
}