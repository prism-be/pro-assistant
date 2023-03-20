using System.Text.Json.Serialization;

namespace Prism.ProAssistant.Api.Models;

public class DownloadReference
{
    [JsonPropertyName("id")]
    required public string Id { get; set; }
}