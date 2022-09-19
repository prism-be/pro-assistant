using System.Net;
using MediatR;

namespace Prism.ProAssistant.Business.Users;

public record AuthenticateUserResult(HttpStatusCode Result, string? Token);

public record AuthenticateUser(string Login, string Password) : IRequest<AuthenticateUserResult>;

public class AuthenticateUserHandler : IRequestHandler<AuthenticateUser, AuthenticateUserResult>
{
    public async Task<AuthenticateUserResult> Handle(AuthenticateUser request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}