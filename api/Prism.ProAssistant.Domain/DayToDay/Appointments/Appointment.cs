namespace Prism.ProAssistant.Domain.DayToDay.Appointments;

using System.Text.Json.Serialization;
using Core.Attributes;

[Collection("appointments")]
public class Appointment : AppointmentInformation
{
    [JsonPropertyName("backgroundColor")]
    public string? BackgroundColor { get; set; }


    [JsonPropertyName("documents")]
    public List<BinaryDocument> Documents { get; set; } = new();


    [JsonPropertyName("foreColor")]
    public string? ForeColor { get; set; }
}