// -----------------------------------------------------------------------
//  <copyright file = "AuthenticateUser.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Net;
using FluentValidation;
using Isopoh.Cryptography.Argon2;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Security;

namespace Prism.ProAssistant.Business.Users;

public record AuthenticateUserResult(HttpStatusCode Result, string? AccessToken, string? RefreshToken);

public record AuthenticateUser(string Login, string Password) : IRequest<AuthenticateUserResult>;

public class AuthenticateUserValidator : AbstractValidator<AuthenticateUser>
{
    public AuthenticateUserValidator()
    {
        RuleFor(x => x.Login).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class AuthenticateUserHandler : IRequestHandler<AuthenticateUser, AuthenticateUserResult>
{
    private readonly IMongoDatabase _database;
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly ILogger<AuthenticateUserHandler> _logger;

    public AuthenticateUserHandler(ILogger<AuthenticateUserHandler> logger, IMongoDatabase database, JwtConfiguration jwtConfiguration)
    {
        _logger = logger;
        _database = database;
        _jwtConfiguration = jwtConfiguration;
    }

    public async Task<AuthenticateUserResult> Handle(AuthenticateUser request, CancellationToken cancellationToken)
    {
        var collection = _database.GetCollection<User>();
        var users = await collection.FindAsync(Builders<User>.Filter.Eq("login", request.Login), cancellationToken: cancellationToken);
        var user = await users.FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (user != null)
        {
            if (Argon2.Verify(user.PasswordHash, request.Password))
            {
                _logger.LogInformation("Authentication succeed for user {userId}", user.Id);

                return new AuthenticateUserResult(HttpStatusCode.OK,
                    TokenGenerator.GenerateAccessToken(_jwtConfiguration.PrivateKey, user),
                    TokenGenerator.GenerateRefreshToken(_jwtConfiguration.PrivateKey, user));
            }

            _logger.LogWarning("Authentication failed for user {userId}", user.Id);

            return new AuthenticateUserResult(HttpStatusCode.NotFound, null, null);
        }

        if (await collection.CountDocumentsAsync(FilterDefinition<User>.Empty, null, cancellationToken) == 0)
        {
            if (request.Login == "admin" && request.Password == "admin")
            {
                _logger.LogWarning("No user found, generate a new admin user with admin as password");

                user = new User(Identifier.GenerateString(), "admin", Argon2.Hash("admin"), "Admin");
                await collection.InsertOneAsync(user, cancellationToken: cancellationToken);

                return new AuthenticateUserResult(HttpStatusCode.OK,
                    TokenGenerator.GenerateAccessToken(_jwtConfiguration.PrivateKey, user),
                    TokenGenerator.GenerateRefreshToken(_jwtConfiguration.PrivateKey, user));
            }
        }

        _logger.LogWarning("Authentication failed for login {login}", request.Login);

        return new AuthenticateUserResult(HttpStatusCode.NotFound, null, null);
    }
}