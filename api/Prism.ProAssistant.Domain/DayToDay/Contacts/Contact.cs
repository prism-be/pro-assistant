// -----------------------------------------------------------------------
//  <copyright file = "Contact.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Prism.Core.Attributes;

namespace Prism.ProAssistant.Domain.DayToDay.Contacts;

[Collection("contacts")]
public class Contact
{
    [JsonPropertyName("birthDate")]
    public string? BirthDate { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("mobileNumber")]
    public string? MobileNumber { get; set; }

    [JsonPropertyName("number")]
    public string? Number { get; set; }

    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("street")]
    public string? Street { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("zipCode")]
    public string? ZipCode { get; set; }

    [JsonPropertyName("id")]
    required public string Id { get; set; }
}