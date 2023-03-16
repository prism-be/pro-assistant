// -----------------------------------------------------------------------
//  <copyright file = "BinaryDocument.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Prism.ProAssistant.Api.Models;

public class BinaryDocument
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("fileName")]
    required public string FileName { get; set; }

    [JsonPropertyName("id")]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    required public string Id { get; set; }

    [JsonPropertyName("title")]
    required public string Title { get; set; }
}