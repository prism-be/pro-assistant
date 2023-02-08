// -----------------------------------------------------------------------
//  <copyright file = "AuthenticationController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Business.Security;

namespace Prism.ProAssistant.Api.Controllers;

public class AuthenticationController : Controller
{
    private readonly IUser _user;

    public AuthenticationController(IUser user)
    {
        _user = user;
    }

    [HttpGet]
    [Route("api/authentication/user")]
    [Authorize]
    public ActionResult<UserInformation> GetUser()
    {
        return Ok(new UserInformation(_user.Name, _user.Organization, _user.IsAuthenticated));
    }
}