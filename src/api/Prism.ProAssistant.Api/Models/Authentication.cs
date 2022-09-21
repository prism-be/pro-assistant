// -----------------------------------------------------------------------
//  <copyright file = "Authentication.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.ProAssistant.Api.Models;

public record UserToken(string AccessToken, string RefreshToken);
public record UserInformation(string Name, bool Authenticated);