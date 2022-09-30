// -----------------------------------------------------------------------
//  <copyright file = "AuthenticationController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Api.Models;

namespace Prism.ProAssistant.Api.Controllers;

public class AuthenticationController : Controller
{
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("api/authentication/user")]
    [Authorize]
    public ActionResult<UserInformation> GetUser()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return StatusCode(200, new UserInformation(User.Identity.Name, User.Identity.IsAuthenticated));
        }

        return StatusCode(200, new UserInformation(null, User.Identity?.IsAuthenticated == true));
    }
}