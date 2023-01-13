// -----------------------------------------------------------------------
//  <copyright file = "DummyRequest.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.ProAssistant.Business.Tests.Behaviors;

namespace Prism.Picshare.Tests;

public record DummyRequest(string Organization, string Login, string Password) : IRequest<DummyResponse>;

public class DummyRequestHandler : IRequestHandler<DummyRequest, DummyResponse>
{

    public Task<DummyResponse> Handle(DummyRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new DummyResponse(Guid.NewGuid().ToString()));
    }
}