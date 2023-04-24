using System.Text.Json;
using Prism.Core;
using Prism.Core.Attributes;

namespace Prism.ProAssistant.Domain;

[Collection("events")]
public class DomainEvent
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    required public string Data { get; set; }

    required public string Id { get; set; }

    required public string StreamId { get; set; }
    required public string StreamType { get; set; }

    required public string Type { get; set; }

    required public string UserId { get; set; }
    
    public T ToEvent<T>()
    {
        return JsonSerializer.Deserialize<T>(Data) ?? throw new InvalidOperationException("The event data could not be deserialized");
    }
    
    public static DomainEvent FromEvent(string streamId, string userId, object e)
    {
        var streamType = "unknown";
        if (e is IDomainEvent domainEvent)
        {
            streamType = domainEvent.StreamType;
        }
        
        return new DomainEvent
        {
            Data = JsonSerializer.Serialize(e),
            Id = Identifier.GenerateString(),
            StreamId = streamId,
            StreamType = streamType,
            Type = e.GetType().Name ?? throw new InvalidOperationException(),
            UserId = userId
        };
    }
}