// -----------------------------------------------------------------------
//  <copyright file = "DocumentConfiguration.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Prism.Core.Attributes;

namespace Prism.ProAssistant.Domain.Configuration.DocumentConfiguration;

[Collection("documents-configuration")]
public class DocumentConfiguration
{

    [JsonPropertyName("body")]
    public string? Body { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("id")]
    required public string Id { get; set; }
}