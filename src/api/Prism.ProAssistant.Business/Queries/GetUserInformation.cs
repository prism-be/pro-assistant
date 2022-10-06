// -----------------------------------------------------------------------
//  <copyright file = "GetUserInformation.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;

namespace Prism.ProAssistant.Business.Queries;

public record GetUserInformation(Guid UserId) : IRequest<UserInformation>;

public class GetUserInformationValidator : AbstractValidator<GetUserInformation>
{
    public GetUserInformationValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}

public record GetUserInformationHandler : IRequestHandler<GetUserInformation, UserInformation>
{
    private readonly ILogger<GetUserInformationHandler> _logger;
    private readonly IMongoDatabase _mongoDatabase;

    public GetUserInformationHandler(ILogger<GetUserInformationHandler> logger, IMongoDatabase mongoDatabase)
    {
        _logger = logger;
        _mongoDatabase = mongoDatabase;
    }

    public async Task<UserInformation> Handle(GetUserInformation request, CancellationToken cancellationToken)
    {
        var userCollection = _mongoDatabase.GetCollection<UserInformation>();
        var userInformations = await userCollection.FindAsync(x => x.Id == request.UserId, cancellationToken: cancellationToken);
        var userInformation = await userInformations.FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (userInformation == null)
        {
            _logger.LogInformation("The user {userId} is not known, creating a new one.", request.UserId);
            userInformation = new UserInformation
            {
                Id = request.UserId
            };
            await userCollection.InsertOneAsync(userInformation, cancellationToken: cancellationToken);
        }

        if (userInformation.Organizations.Count == 0)
        {
            _logger.LogInformation("The user {userId} has no organisation, adding a new one.", request.UserId);
            userInformation.Organizations = new List<Organization>
            {
                new()
                {
                    Id = Identifier.Generate()
                }
            };

            userCollection.FindOneAndReplace(x => x.Id == request.UserId, userInformation);
        }

        return userInformation;
    }
}