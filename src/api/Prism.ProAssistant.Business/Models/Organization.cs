// -----------------------------------------------------------------------
//  <copyright file = "Organization.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.ProAssistant.Business.Models;

public class Organization
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
}