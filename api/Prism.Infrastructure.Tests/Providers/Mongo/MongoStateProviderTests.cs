using FluentAssertions;
using MongoDB.Driver;
using Moq;
using Prism.Infrastructure.Authentication;
using Prism.Infrastructure.Providers.Mongo;

namespace Prism.Infrastructure.Tests.Providers.Mongo;

public class MongoStateProviderTests
{
    [Fact]
    public async Task GetContainerAsync_Ok()
    {
        // Arrange
        var userOrganization = new UserOrganization();
        var mongoClient = new Mock<IMongoClient>();
        var database = new Mock<IMongoDatabase>();
        var collection = new Mock<IMongoCollection<UserOrganization>>();
        database.Setup(x => x.GetCollection<UserOrganization>("users", null)).Returns(collection.Object);
        mongoClient.Setup(x => x.GetDatabase(userOrganization.Organization, null)).Returns(database.Object);
        
        // Act
        var mongoStateProvider = new MongoStateProvider(mongoClient.Object, userOrganization);
        var result = await mongoStateProvider.GetContainerAsync<UserOrganization>();

        // Assert
        result.Should().NotBeNull();
    }
}