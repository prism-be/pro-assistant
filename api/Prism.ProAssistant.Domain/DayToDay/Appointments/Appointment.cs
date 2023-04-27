﻿using System.Text.Json.Serialization;
using Prism.Core.Attributes;

namespace Prism.ProAssistant.Domain.DayToDay.Appointments;

[Collection("appointments")]
public class Appointment
{
    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }

    [JsonPropertyName("paymentDate")]
    public DateTime? PaymentDate { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [JsonPropertyName("payment")]
    public int Payment { get; set; }

    [JsonPropertyName("state")]
    public int State { get; set; }

    [JsonPropertyName("documents")]
    public List<BinaryDocument> Documents { get; set; } = new();

    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("id")]
    required public string Id { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("backgroundColor")]
    public string? BackgroundColor { get; set; }

    [JsonPropertyName("birthDate")]
    public string? BirthDate { get; set; }

    [JsonPropertyName("contactId")]
    public string? ContactId { get; set; }

    [JsonPropertyName("foreColor")]
    public string? ForeColor { get; set; }

    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("typeId")]
    public string? TypeId { get; set; }
}

public enum AppointmentState
{
    Created = 0,
    Confirmed = 1,
    Done = 10,
    Canceled = 100
}

public enum PaymentTypes
{
    Unpayed = 0,
    Cash = 1,
    Wire = 2,
    Card = 3
}