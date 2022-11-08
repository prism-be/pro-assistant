// -----------------------------------------------------------------------
//  <copyright file = "Tariff.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Prism.ProAssistant.Business.Models;

[BsonCollection("tariffs")]
public class Tariff : IDataModel
{

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("defaultDuration")]
    public int DefaultDuration { get; set; } = 60;

    [JsonPropertyName("id")]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}