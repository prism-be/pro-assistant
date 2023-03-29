using Prism.ProAssistant.Api.Models;

namespace Prism.ProAssistant.Api.Services;

public class Event<T> where T : IDataModel
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public EventType EventType { get; set; }
    public string Id { get; set; } = Identifier.GenerateString();
    required public string ObjectId { get; set; }
    public string? UserId { get; set; }
    public T? Data { get; set; }
    public KeyValuePair<string, object>[]? Updates { get; set; }
}

public enum EventType
{
    Insert = 0,
    Update = 1,
    Replace = 2,
    Delete = 3
}