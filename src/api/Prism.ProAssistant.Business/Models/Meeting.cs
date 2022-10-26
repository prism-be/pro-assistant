﻿// -----------------------------------------------------------------------
//  <copyright file = "Meeting.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Prism.ProAssistant.Business.Models;

public class Meeting
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("patientId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? PatientId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }

    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [JsonPropertyName("state")]
    public int State { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("payment")]
    public int Payment { get; set; }

    [JsonPropertyName("paymentDate")]
    public DateTime? PaymentDate { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}

public enum MeetingState
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