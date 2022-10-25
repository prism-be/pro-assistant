// -----------------------------------------------------------------------
//  <copyright file = "History.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Prism.ProAssistant.Business.Security;

namespace Prism.ProAssistant.Business.Models;

public class History
{
    public History()
    {
        Id = string.Empty;
    }
    
    public History(string userId, object actual)
    {
        Actual = actual;
        UserId = userId;
        ModificationDate = DateTime.UtcNow;
        Id = string.Empty;
        TypeName = actual.GetType().FullName;
    }

    [JsonPropertyName("modificationDate")]
    public DateTime ModificationDate { get; set; }

    [JsonPropertyName("actual")]
    public object? Actual { get; set; }

    [JsonPropertyName("id")]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    [JsonPropertyName("typeName")]
    public string? TypeName { get; set; }
}