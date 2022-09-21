// -----------------------------------------------------------------------
//  <copyright file = "AuthenticationController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Users;

namespace Prism.ProAssistant.Api.Controllers;

public class AuthenticationController : Controller
{
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("authentication/login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(AuthenticateUser request)
    {
        // var result = await _mediator.Send(request);

        var result = new AuthenticateUserResult(HttpStatusCode.OK, TokenGenerator.GenerateAccessToken(
            "MIICWwIBAAKBgQCTEIeSizY8CY9tl5KTpf6tjdt8HqZ2NcvctsxoKuc1kIr+dlCFWoDpdFH1xlqFSoXnq07L6B0z2SdKCQroiolNsRFXOM6ztVEDMLqrgwPxOkm7pl2DjAOQ/dW9n4YcHUCgAEilYJGfBl/i4h1X8OFRY7Lnb2PbMuJSfSu4iM/S3QIDAQABAoGADRP3Omzu0b/35UMJYd/tGfn3fr4rB3AZRPNskgbesMC924sh9fnqZNhXQYf2HMxXxBZT14Y4spepCshrE+rd8ssfb04c/QSZahx0YYVx0ZpRO+RDukncym4miFGeWsRVs6VPPs377lyFhTarqXplzHhN7xFmv5uu+a9Q8AJ9OoECQQD6Hcld0EJs29os5e4ROSFwxvEXnjxVHwUDnAE/CUbXGGqX+hDBsRCvHSqDHdfsXkHtbgYDAM4OEDbFUxJakRr1AkEAloYo7wLfjhrnvzJ6mNPQe+arZGta/8Mmd6KytOIydFi0IhMDFYhWcn/26cPuF98AyUVva8evYxT9v3ZI6EK3SQJAMcryRqnqP2+5TIztyyH7hU8luhT3X97QCbrqCJmZL8MdnWncIiNU9fexee7cCKNvLoxjx/9GBki2DqyOD9epbQJAS/tpCk2O/7LBLRiUHjU91m14MvamNtkRv+5W+0v7YBOuykyCkAoEaUQDJbmLpG0jfnYYanWgKQndlRpmbyfOUQJAXD6FC+67qqjvPR/ZeNhZ2p2e+DU3z3Mqtj/kYQbno9+R/8oALZptj6Go1n8Sk5AvYzcao0/flfnr2TwYEV8ArA==",
            new User
            ("fake","admin", "Admin", string.Empty)));

        return StatusCode((int)result.Result, result);
    }
}