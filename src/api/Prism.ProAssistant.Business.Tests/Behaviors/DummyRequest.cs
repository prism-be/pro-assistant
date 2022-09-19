// -----------------------------------------------------------------------
//  <copyright file = "DummyRequest.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Prism.Picshare.Tests;

public record DummyRequest(string Organisation, string Login, string Password) : IRequest<DummyResponse>;

public class DummyRequestHandler : IRequestHandler<DummyRequest, DummyResponse>
{

    public Task<DummyResponse> Handle(DummyRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new DummyResponse(Guid.NewGuid().ToString()));
    }
}