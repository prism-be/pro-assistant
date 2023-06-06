namespace Prism.ProAssistant.Api.Models;

using System.Text.Json.Serialization;

public class AppointmentClosing
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("payment")]
    public int Payment { get; set; }

    [JsonPropertyName("paymentDate")]
    public DateTime? PaymentDate { get; set; }

    [JsonPropertyName("state")]
    public int State { get; set; }
}