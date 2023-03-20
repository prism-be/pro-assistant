using System.Text.Json.Serialization;

namespace Prism.ProAssistant.Api.Models;

public class DocumentRequest
{
    [JsonPropertyName("documentId")]
    required public string DocumentId { get; set; }

    [JsonPropertyName("appointmentId")]
    required public string AppointmentId { get; set; }
}