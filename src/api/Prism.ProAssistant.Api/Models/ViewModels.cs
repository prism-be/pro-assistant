// -----------------------------------------------------------------------
//  <copyright file = "ViewModels.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.ProAssistant.Api.Models;

public record SearchAppointments(DateTime StartDate, DateTime EndDate, string? ContactId);

public record SearchContacts(string LastName, string FirstName, string PhoneNumber, string BirthDate);

public record DeleteDocument(string Id, string AppointmentId);

public record DownloadDocument(string DocumentId);

public record GenerateDocument(string DocumentId, string AppointmentId);