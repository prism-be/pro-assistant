// -----------------------------------------------------------------------
//  <copyright file = "Authentication.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.ProAssistant.Api.Models;

public record UserInformation(string? Name, string? Organization, bool Authenticated);