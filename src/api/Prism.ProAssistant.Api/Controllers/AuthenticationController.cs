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
    private readonly IMediator _mediator;
    private readonly IUserContextAccessor _userContextAccessor;

    public AuthenticationController(IMediator mediator, IUserContextAccessor userContextAccessor)
    {
        _mediator = mediator;
        _userContextAccessor = userContextAccessor;
    }

    [HttpGet]
    [Route("api/authentication/user")]
    [Authorize]
    public ActionResult<UserInformation> GetUser()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return StatusCode(200, new UserInformation(_userContextAccessor.Name, User.Identity.IsAuthenticated));
        }

        return StatusCode(200, new UserInformation(null, User.Identity?.IsAuthenticated == true));
    }
}