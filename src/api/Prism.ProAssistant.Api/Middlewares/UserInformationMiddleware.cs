// -----------------------------------------------------------------------
//  <copyright file = "UserInformationMiddleware.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Security.Claims;
using Prism.ProAssistant.Business.Security;

namespace Prism.ProAssistant.Api.Middlewares;

public class UserInformationMiddleware
{
    
    private readonly RequestDelegate _next;

    public UserInformationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    ///     Invoke the middleware.
    /// </summary>
    /// <param name="httpContext">The current http context.</param>
    /// <returns>The task to be waited.</returns>
    public async Task InvokeAsync(HttpContext httpContext, IUser _user)
    {
        _user.IsAuthenticated = httpContext.User.Identity?.IsAuthenticated == true;

        if (_user.IsAuthenticated)
        {
            _user.Organization = httpContext.User.Claims.FirstOrDefault(x => x.Type == "extension_Organization")?.Value;
            _user.Name = httpContext.User.Claims.FirstOrDefault(x => x.Type == "name")?.Value;
            _user.Id = httpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        await _next(httpContext);
    }
}