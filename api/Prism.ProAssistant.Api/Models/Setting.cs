// -----------------------------------------------------------------------
//  <copyright file = "Setting.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.ProAssistant.Api.Models;

[BsonCollection("settings")]
public class Setting : IDataModel
{
    [JsonPropertyName("value")]
    required public string Value { get; set; }

    [JsonPropertyName("id")]
    required public string Id { get; set; }
}