using FluentAssertions;
using MongoDB.Driver;
using Moq;
using Prism.Infrastructure.Authentication;
using Prism.Infrastructure.Providers.Mongo;

namespace Prism.Infrastructure.Tests.Providers.Mongo;

public class MongoGlobalStateProviderTests
{
    [Fact]
    public async Task GetGlobalContainerAsync_Ok()
    {
        // Arrange
        var mongoClient = new Mock<IMongoClient>();
        var database = new Mock<IMongoDatabase>();
        var collection = new Mock<IMongoCollection<UserOrganization>>();
        
        database.Setup(x => x.GetCollection<UserOrganization>("users", null)).Returns(collection.Object);
        mongoClient.Setup(x => x.GetDatabase("_global", null)).Returns(database.Object);
        
        // Act
        var mongoGlobalStateProvider = new MongoGlobalStateProvider(mongoClient.Object);
        var result = await mongoGlobalStateProvider.GetGlobalContainerAsync<UserOrganization>();

        // Assert
        result.Should().NotBeNull();
    }
}