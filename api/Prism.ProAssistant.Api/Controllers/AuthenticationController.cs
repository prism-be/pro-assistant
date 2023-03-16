using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Prism.ProAssistant.Api.Controllers;

[Authorize]
public class AuthenticationController
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationController(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet]
    [Route("api/authentication/user")]
    public ActionResult GetUser()
    {
        return new OkObjectResult(new
        {
            IsAuthenticated = true,
            Name = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "name")?.Value,
        });
    }
}