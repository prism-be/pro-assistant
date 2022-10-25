// -----------------------------------------------------------------------
//  <copyright file = "UserInformation.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Prism.ProAssistant.Business.Models;

public class UserInformation
{
    public UserInformation()
    {
        Organizations = new List<Organization>();
    }

    [JsonPropertyName("id")]
    [BsonId]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("organizations")]
    public List<Organization> Organizations { get; set; }
}