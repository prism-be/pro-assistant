// -----------------------------------------------------------------------
//  <copyright file = "UserContextAccessor.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Prism.ProAssistant.Business.Security;

public interface IUserContextAccessor
{
    bool IsAuthenticated { get; }
    string Name { get; }
    string OrganizationId { get; }
    string UserId { get; }
}

public class UserContextAccessor : IUserContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;

    public string OrganizationId
    {
        get
        {
            if (!IsAuthenticated)
            {
                return string.Empty;
            }

            var organization = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "extension_Organization")?.Value;

            if (string.IsNullOrWhiteSpace(organization))
            {
                throw new AuthenticationException($"The user {UserId} don't have an organization");
            }

            return organization;
        }
    }

    public string Name
    {
        get
        {
            if (!IsAuthenticated)
            {
                return string.Empty;
            }

            var name = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "name");

            if (name == null)
            {
                return string.Empty;
            }

            return name.Value;
        }
    }

    public string UserId
    {
        get
        {
            if (_httpContextAccessor.HttpContext.User.Identity?.IsAuthenticated == false)
            {
                return string.Empty;
            }

            var objectid = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (objectid == null)
            {
                return string.Empty;
            }

            return objectid.Value;
        }
    }
}