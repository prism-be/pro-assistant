// -----------------------------------------------------------------------
//  <copyright file = "DocumentPdf.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.ProAssistant.Business.Models;

public class Document
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public byte[] Data { get; set; } = Array.Empty<byte>();

    [JsonPropertyName("contactId")]
    public string ContactId { get; set; } = string.Empty;

    [JsonPropertyName("creationDate")]
    public DateTime CreationDate { get; set; } = DateTime.Now;

    [JsonPropertyName("creatorId")]
    public string CreatorId { get; set; } = string.Empty;

    [JsonPropertyName("documentConfigurationId")]
    public string DocumentConfigurationId { get; set; } = string.Empty;

    [JsonPropertyName("documentConfiguration")]
    public string DocumentConfiguration { get; set; } = string.Empty;
}