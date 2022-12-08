// -----------------------------------------------------------------------
//  <copyright file = "UpdatePatientMeetings.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.ProAssistant.Business.Models;

namespace Prism.ProAssistant.Business.Commands;

public record UpdatePatientMeetings(Meeting Previous, Meeting Current): IRequest;