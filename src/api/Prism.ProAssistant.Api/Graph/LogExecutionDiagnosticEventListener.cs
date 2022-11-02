// -----------------------------------------------------------------------
//  <copyright file = "LogExecutionDiagnosticEventListener.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using HotChocolate.Execution;
using HotChocolate.Execution.Instrumentation;
using HotChocolate.Resolvers;

namespace Prism.ProAssistant.Api.Graph;

public class LogExecutionDiagnosticEventListener : ExecutionDiagnosticEventListener
{
    private readonly ILogger<LogExecutionDiagnosticEventListener> _logger;

    public LogExecutionDiagnosticEventListener(ILogger<LogExecutionDiagnosticEventListener> logger)
    {
        _logger = logger;
    }

    public override void RequestError(IRequestContext context, Exception exception)
    {
        _logger.LogError(exception, "GraphQL error while processing a graphql command");
        base.RequestError(context, exception);
    }

    public override void ResolverError(IMiddlewareContext context, IError error)
    {
        _logger.LogWarning(error.Exception, "GraphQL error while resolving : {graphqlError}", error.Message);
        base.ResolverError(context, error);
    }

    public override void SyntaxError(IRequestContext context, IError error)
    {
        _logger.LogWarning(error.Exception, "GraphQL error while checking syntax : {graphqlError}", error.Message);
        base.SyntaxError(context, error);
    }

    public override void ValidationErrors(IRequestContext context, IReadOnlyList<IError> errors)
    {
        foreach (var error in errors)
        {
            _logger.LogWarning(error.Exception, "GraphQL error while validating syntax : {graphqlError}", error.Message);
        }

        base.ValidationErrors(context, errors);
    }
}