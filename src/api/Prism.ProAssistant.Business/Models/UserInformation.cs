// -----------------------------------------------------------------------
//  <copyright file = "UserInformation.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.ProAssistant.Business.Models;

public class UserInformation
{
    public UserInformation()
    {
        Organizations = new List<Organization>();
    }

    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("organizations")]
    public List<Organization> Organizations { get; set; }
}