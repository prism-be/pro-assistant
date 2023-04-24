using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.Infrastructure.Authentication;
using Prism.ProAssistant.Api.Models;

namespace Prism.ProAssistant.Api.Controllers;

[Authorize]
public class AuthenticationController
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserOrganization _userOrganization;

    public AuthenticationController(IHttpContextAccessor httpContextAccessor, UserOrganization userOrganization)
    {
        _httpContextAccessor = httpContextAccessor;
        _userOrganization = userOrganization;
    }

    [HttpGet]
    [Route("api/authentication/user")]
    public UserInformation GetUser()
    {
        return new UserInformation
        {
            IsAuthenticated = true,
            Name = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "name")?.Value ?? string.Empty,
            Organization = _userOrganization.Organization
        };
    }
}