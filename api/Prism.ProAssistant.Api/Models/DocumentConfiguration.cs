// -----------------------------------------------------------------------
//  <copyright file = "DocumentConfiguration.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Prism.ProAssistant.Api.Models;

[BsonCollection("documents-configuration")]
public class DocumentConfiguration : IDataModel
{

    [JsonPropertyName("body")]
    public string? Body { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonPropertyName("id")]
    required public string Id { get; set; }
}