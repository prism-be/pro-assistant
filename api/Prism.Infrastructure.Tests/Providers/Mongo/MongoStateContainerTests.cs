using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Prism.Infrastructure.Authentication;
using Prism.Infrastructure.Providers;
using Prism.Infrastructure.Providers.Mongo;

namespace Prism.Infrastructure.Tests.Providers.Mongo;

public class MongoStateContainerTests
{
    [Fact]
    public async Task ReadAsync_Ok()
    {
        // Arrange
        var userOrganization = new UserOrganization
        {
            Id = ObjectId.GenerateNewId().ToString()
        };

        var collection = new Mock<IMongoCollection<UserOrganization>>();
        collection.SetupCollection(userOrganization);
        
        // Act
        var mongoStateContainer = new MongoStateContainer<UserOrganization>(collection.Object);
        var result = await mongoStateContainer.ReadAsync(userOrganization.Id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(userOrganization);
    }

    [Fact]
    public async Task WriteAsync_Ok()
    {
        // Arrange
        var userOrganization = new UserOrganization
        {
            Id = ObjectId.GenerateNewId().ToString()
        };
        
        var collection = new Mock<IMongoCollection<UserOrganization>>();
        
        // Act
        var mongoStateContainer = new MongoStateContainer<UserOrganization>(collection.Object);
        await mongoStateContainer.WriteAsync(userOrganization.Id, userOrganization);

        // Assert
        collection.Verify(x => x.ReplaceOneAsync(
            It.IsAny<FilterDefinition<UserOrganization>>(),
            It.Is<UserOrganization>(y => y.Id == userOrganization.Id),
            It.IsAny<ReplaceOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FetchAsync_Ok()
    {
        // Arrange
        var userOrganization1 = new UserOrganization
        {
            Id = ObjectId.GenerateNewId().ToString()
        };
        
        var userOrganization2 = new UserOrganization
        {
            Id = ObjectId.GenerateNewId().ToString()
        };
        
        var collection = new Mock<IMongoCollection<UserOrganization>>();
        collection.SetupCollection(userOrganization1, userOrganization2);
        
        // Act
        var mongoStateContainer = new MongoStateContainer<UserOrganization>(collection.Object);
        var result = await mongoStateContainer.FetchAsync();

        // Assert
        var userOrganizations = result as UserOrganization[] ?? result.ToArray();
        userOrganizations.Should().NotBeNull();
        userOrganizations.Should().BeEquivalentTo(new[] {userOrganization1, userOrganization2});
    }
    
    [Fact]
    public async Task FetchAsync_WithFiltersUnknown()
    {
        // Arrange
        var userOrganization1 = new UserOrganization
        {
            Id = ObjectId.GenerateNewId().ToString()
        };
        
        var userOrganization2 = new UserOrganization
        {
            Id = ObjectId.GenerateNewId().ToString()
        };
        
        var collection = new Mock<IMongoCollection<UserOrganization>>();
        collection.SetupCollection(userOrganization1, userOrganization2);
        
        // Act
        var mongoStateContainer = new MongoStateContainer<UserOrganization>(collection.Object);
        var action = async () => await mongoStateContainer.FetchAsync(
            new Filter(nameof(UserOrganization.Id), userOrganization1.Id, "ThisDoesNotExists")
        );

        // Assert
        await action.Should().ThrowAsync<NotSupportedException>();
    }
    
    [Fact]
    public async Task FetchAsync_WithFilters()
    {
        // Arrange
        var userOrganization1 = new UserOrganization
        {
            Id = ObjectId.GenerateNewId().ToString()
        };
        
        var userOrganization2 = new UserOrganization
        {
            Id = ObjectId.GenerateNewId().ToString()
        };
        
        var collection = new Mock<IMongoCollection<UserOrganization>>();
        collection.SetupCollection(userOrganization1, userOrganization2);
        
        // Act
        var mongoStateContainer = new MongoStateContainer<UserOrganization>(collection.Object);
        var result = await mongoStateContainer.FetchAsync(
                new Filter(nameof(UserOrganization.Id), userOrganization1.Id)
            );

        // Assert
        var userOrganizations = result as UserOrganization[] ?? result.ToArray();
        userOrganizations.Should().NotBeNull();
        userOrganizations.Should().BeEquivalentTo(new[] {userOrganization1, userOrganization2});
    }
}