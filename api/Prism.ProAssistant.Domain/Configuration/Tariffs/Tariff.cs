using System.Text.Json.Serialization;
using Prism.Core.Attributes;

namespace Prism.ProAssistant.Domain.Configuration.Tariffs;

using Core;

[Collection("tariffs")]
public class Tariff
{

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("defaultDuration")]
    public int DefaultDuration { get; set; } = 60;

    [JsonPropertyName("id")]
    public string Id { get; set; } = Identifier.GenerateString();

    [JsonPropertyName("name")]
    required public string Name { get; set; }

    [JsonPropertyName("backgroundColor")]
    public string? BackgroundColor { get; set; }

    [JsonPropertyName("foreColor")]
    public string? ForeColor { get; set; }
}