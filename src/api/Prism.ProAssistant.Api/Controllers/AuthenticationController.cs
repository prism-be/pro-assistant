// -----------------------------------------------------------------------
//  <copyright file = "AuthenticationController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Business.Security;

namespace Prism.ProAssistant.Api.Controllers;

public class AuthenticationController : Controller
{
    private readonly IUserContextAccessor _userContextAccessor;

    public AuthenticationController(IUserContextAccessor userContextAccessor)
    {
        _userContextAccessor = userContextAccessor;
    }

    [HttpGet]
    [Route("api/authentication/user")]
    [Authorize]
    public ActionResult<UserInformation> GetUser()
    {
        if (_userContextAccessor.IsAuthenticated)
        {
            return Ok(new UserInformation(_userContextAccessor.Name, true));
        }

        return Ok(new UserInformation(null, false));
    }
}