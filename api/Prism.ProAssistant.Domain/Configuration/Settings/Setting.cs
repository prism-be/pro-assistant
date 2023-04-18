using System.Text.Json.Serialization;
using Prism.Core.Attributes;

namespace Prism.ProAssistant.Domain.Configuration.Settings;

[Collection("settings")]
public class Setting
{

    [JsonPropertyName("id")]
    required public string Id { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}