// -----------------------------------------------------------------------
//  <copyright file = "UserContextAccessor.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Prism.ProAssistant.Business.Security;

public interface IUserContextAccessor
{
    bool IsAuthenticated { get; }
    Guid UserId { get; }
    string Name { get; }
}

public class UserContextAccessor : IUserContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext.User.Identity?.IsAuthenticated == true;

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

    public Guid UserId
    {
        get
        {
            if (_httpContextAccessor.HttpContext.User.Identity?.IsAuthenticated == false)
            {
                return Guid.Empty;
            }

            var objectid = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (objectid == null)
            {
                return Guid.Empty;
            }

            return Guid.Parse(objectid.Value);
        }
    }
}