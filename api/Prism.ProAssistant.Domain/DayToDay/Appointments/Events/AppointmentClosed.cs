namespace Prism.ProAssistant.Domain.DayToDay.Appointments.Events;

using System.Text.Json.Serialization;
using Core.Attributes;

[StreamType(Streams.Appointments)]
public class AppointmentClosed : BaseEvent
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("payment")]
    public int Payment { get; set; }
    
    [JsonPropertyName("paymentDate")]
    public DateTime? PaymentDate { get; set; }

    [JsonPropertyName("state")]
    public int State { get; set; }

    public override string StreamId => Id;
}