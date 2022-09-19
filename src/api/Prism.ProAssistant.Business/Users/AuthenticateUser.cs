using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Prism.ProAssistant.Business.Users;

public record AuthenticateUserResult(HttpStatusCode Result, string? Token);

public record AuthenticateUser(string Login, string Password) : IRequest<AuthenticateUserResult>;

public class AuthenticateUserHandler : IRequestHandler<AuthenticateUser, AuthenticateUserResult>
{
    private readonly ILogger<AuthenticateUserHandler> _logger;
    private readonly IMongoDatabase _database;

    public AuthenticateUserHandler(ILogger<AuthenticateUserHandler> logger, IMongoDatabase database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task<AuthenticateUserResult> Handle(AuthenticateUser request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}