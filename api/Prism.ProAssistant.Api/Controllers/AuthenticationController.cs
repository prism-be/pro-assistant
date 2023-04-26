using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.Infrastructure.Authentication;
using Prism.ProAssistant.Api.Models;

namespace Prism.ProAssistant.Api.Controllers;

[Authorize]
public class AuthenticationController
{
    private readonly UserOrganization _userOrganization;

    public AuthenticationController(UserOrganization userOrganization)
    {
        _userOrganization = userOrganization;
    }

    [HttpGet]
    [Route("api/authentication/user")]
    public UserInformation GetUser()
    {
        return new UserInformation
        {
            IsAuthenticated = true,
            Name = _userOrganization.Name,
            Organization = _userOrganization.Organization
        };
    }
}