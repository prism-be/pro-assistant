using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;

namespace Prism.ProAssistant.Api.Controllers;

[Authorize]
public class AuthenticationController
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserOrganizationService _userOrganizationService;

    public AuthenticationController(IHttpContextAccessor httpContextAccessor, IUserOrganizationService userOrganizationService)
    {
        _httpContextAccessor = httpContextAccessor;
        _userOrganizationService = userOrganizationService;
    }

    [HttpGet]
    [Route("api/authentication/user")]
    public async Task<UserInformation> GetUser()
    {
        return new UserInformation
        {
            IsAuthenticated = true,
            Name = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "name")?.Value ?? string.Empty,
            Organization = await _userOrganizationService.GetUserOrganization()
        };
    }
}

