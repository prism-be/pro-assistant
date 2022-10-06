// -----------------------------------------------------------------------
//  <copyright file = "GetUserInformationTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Queries;
using Prism.ProAssistant.Business.Security;

namespace Prism.ProAssistant.Business.Tests.Queries;

public class GetUserInformationTests
{
    [Fact]
    public async Task Handle_Unknwon_User()
    {
        // Arrange
        var userId = Identifier.Generate();

        var database = new Mock<IMongoDatabase>();
        database.SetupCollectionFindEmpty<UserInformation>();

        var handler = new GetUserInformationHandler(Mock.Of<ILogger<GetUserInformationHandler>>(), database.Object);

        // Act
        var result = await handler.Handle(new GetUserInformation(userId), CancellationToken.None);

        // Assert
        result.Id.Should().Be(userId);
        result.Organizations.Count.Should().Be(1);
    }
    
    [Fact]
    public async Task Handle_Classic()
    {
        // Arrange
        var userId = Identifier.Generate();

        var database = new Mock<IMongoDatabase>();
        database.SetupCollection(new UserInformation
        {
            Id = userId,
            Organizations = new List<Organization>
            {
                new()
                {
                    Id = Identifier.Generate()
                }
            }
        });

        var handler = new GetUserInformationHandler(Mock.Of<ILogger<GetUserInformationHandler>>(), database.Object);

        // Act
        var result = await handler.Handle(new GetUserInformation(userId), CancellationToken.None);

        // Assert
        result.Id.Should().Be(userId);
        result.Organizations.Count.Should().Be(1);
    }

    [Fact]
    public void Validate_Empty()
    {
        // Arrange
        var request = new GetUserInformation(Guid.Empty);

        // Act
        var valdiation = new GetUserInformationValidator().Validate(request);

        // Assert
        valdiation.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_Ok()
    {
        // Arrange
        var request = new GetUserInformation(Identifier.Generate());

        // Act
        var valdiation = new GetUserInformationValidator().Validate(request);

        // Assert
        valdiation.IsValid.Should().BeTrue();
    }
}