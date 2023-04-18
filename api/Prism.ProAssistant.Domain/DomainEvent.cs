using System.Text.Json;
using Prism.Core.Attributes;

namespace Prism.ProAssistant.Domain;

[Collection("events")]
public class DomainEvent
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    required public string Data { get; set; }

    required public string Id { get; set; }

    required public string StreamId { get; set; }

    required public string Type { get; set; }

    required public string UserId { get; set; }
    
    public T ToObject<T>()
    {
        return JsonSerializer.Deserialize<T>(Data) ?? throw new InvalidOperationException("The event data could not be deserialized");
    }
}