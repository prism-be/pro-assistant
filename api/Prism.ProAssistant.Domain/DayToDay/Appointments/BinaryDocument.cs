using System.Text.Json.Serialization;

namespace Prism.ProAssistant.Domain.DayToDay.Appointments;

public class BinaryDocument
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("fileName")]
    required public string FileName { get; set; }

    [JsonPropertyName("id")]
    required public string Id { get; set; }

    [JsonPropertyName("title")]
    required public string Title { get; set; }
}