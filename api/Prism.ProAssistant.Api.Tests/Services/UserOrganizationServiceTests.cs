using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Api.Exceptions;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;

namespace Prism.ProAssistant.Api.Tests.Services;

public class UserOrganizationServiceTests
{

    [Fact]
    public async Task GetUserCollection_NoId()
    {
        // Arrange
        var cache = new Mock<IDistributedCache>();
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("name", "Test User")
            }, "Test Authentication Type"))
        };
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
        var logger = new Mock<ILogger<UserOrganizationService>>();
        var database = new Mock<IMongoDatabase>();
        var mongoClient = new Mock<IMongoClient>();
        mongoClient.Setup(x => x.GetDatabase(It.IsAny<string>(), null)).Returns(database.Object);

        // Act
        var service = new UserOrganizationService(logger.Object, httpContextAccessor.Object, mongoClient.Object, cache.Object);
        await service.Invoking(x => x.GetUserCollection<Contact>()).Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetUserCollection_Ok()
    {
        // Arrange
        var userId = Identifier.GenerateString();
        var cache = new Mock<IDistributedCache>();
        cache.Setup(x => x.GetAsync($"organization-{userId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as byte[]);
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("name", "Test User"),
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "Test Authentication Type"))
        };
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
        var logger = new Mock<ILogger<UserOrganizationService>>();
        var database = new Mock<IMongoDatabase>();

        var usersCollection = new Mock<IMongoCollection<BsonDocument>>();
        usersCollection.SetupCollection(new BsonDocument(new List<KeyValuePair<string, object>>
        {
            new("organization", "tests")
        }));
        database.Setup(x => x.GetCollection<BsonDocument>("users", null)).Returns(usersCollection.Object);

        var mongoClient = new Mock<IMongoClient>();
        mongoClient.Setup(x => x.GetDatabase(It.IsAny<string>(), null)).Returns(database.Object);

        // Act
        var service = new UserOrganizationService(logger.Object, httpContextAccessor.Object, mongoClient.Object, cache.Object);
        var result = await service.GetUserCollection<Contact>();

        // Assert
        mongoClient.Verify(x => x.GetDatabase(It.IsAny<string>(), null), Times.Exactly(2));
    }

    [Fact]
    public async Task GetUserCollection_Unauthenticated()
    {
        // Arrange
        var cache = new Mock<IDistributedCache>();
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        var logger = new Mock<ILogger<UserOrganizationService>>();
        var database = new Mock<IMongoDatabase>();
        var mongoClient = new Mock<IMongoClient>();
        mongoClient.Setup(x => x.GetDatabase(It.IsAny<string>(), null)).Returns(database.Object);

        // Act
        var service = new UserOrganizationService(logger.Object, httpContextAccessor.Object, mongoClient.Object, cache.Object);
        await service.Invoking(x => x.GetUserCollection<Contact>()).Should().ThrowAsync<NotFoundException>();
    }
}