// -----------------------------------------------------------------------
//  <copyright file = "AuthenticateUserTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Net;
using FluentAssertions;
using Isopoh.Cryptography.Argon2;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Users;

namespace Prism.ProAssistant.Tests.Users;

public class AuthenticateUserTests
{
    [Fact]
    public async Task Handle_Admin_Ok_WithPassword()
    {
        // Arrange
        var password = Identifier.GenerateString();
        var request = new AuthenticateUser("admin", password);
        var user = new User(Identifier.GenerateString(), "admin", Argon2.Hash(password, 1, 42), "Admin");
        var cursor = new Mock<IAsyncCursor<User>>();
        var items = new List<User>
        {
            user
        };
        cursor.Setup(_ => _.Current).Returns(items);
        cursor
            .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);
        var collection = new Mock<IMongoCollection<User>>();
        collection.Setup(x => x.FindAsync(It.IsAny<FilterDefinition<User>>(), null, CancellationToken.None))
            .ReturnsAsync(cursor.Object);
        var database = new Mock<IMongoDatabase>();
        database.Setup(x => x.GetCollection<User>(nameof(User).ToLowerInvariant(), null)).Returns(collection.Object);

        // Act
        var handler = new AuthenticateUserHandler(Mock.Of<ILogger<AuthenticateUserHandler>>(), database.Object);
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Result.Should().Be(HttpStatusCode.OK);
        result.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Handle_Admin_Ok()
    {
        // Arrange
        var request = new AuthenticateUser("admin", "admin");
        var database = new Mock<IMongoDatabase>();

        // Act
        var handler = new AuthenticateUserHandler(Mock.Of<ILogger<AuthenticateUserHandler>>(), database.Object);
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Result.Should().Be(HttpStatusCode.OK);
        result.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Handle_Admin_Ko_WhenOthers()
    {
        // Arrange
        var request = new AuthenticateUser("admin", "admin");
        var collection = new Mock<IMongoCollection<User>>();
        collection.Setup(x => x.CountDocumentsAsync(FilterDefinition<User>.Empty, null, CancellationToken.None)).ReturnsAsync(42);
        var database = new Mock<IMongoDatabase>();
        database.Setup(x => x.GetCollection<User>("user", null)).Returns(collection.Object);

        // Act
        var handler = new AuthenticateUserHandler(Mock.Of<ILogger<AuthenticateUserHandler>>(), database.Object);
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Result.Should().Be(HttpStatusCode.Forbidden);
        result.Token.Should().BeNull();
    }
}