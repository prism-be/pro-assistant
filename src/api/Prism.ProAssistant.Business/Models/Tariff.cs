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

    [JsonPropertyName("name")]
    required public string Name { get; set; }

    [JsonPropertyName("backgroundColor")]
    public string? BackgroundColor { get; set; }

    [JsonPropertyName("foreColor")]
    public string? ForeColor { get; set; }

    [JsonPropertyName("id")]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    required public string Id { get; set; }
}