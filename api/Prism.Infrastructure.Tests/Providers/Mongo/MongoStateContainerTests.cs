namespace Prism.Infrastructure.Tests.Providers.Mongo;

using System.Globalization;
using Core;
using FluentAssertions;
using Infrastructure.Authentication;
using Infrastructure.Providers;
using Infrastructure.Providers.Mongo;
using MongoDB.Driver;
using Moq;

public class MongoStateContainerTests
{
    [Fact]
    public async Task DeleteAsync_Ok()
    {
        // Arrange
        var userOrganization = new UserOrganization
        {
            Id = Identifier.GenerateString()
        };

        var collection = new Mock<IMongoCollection<UserOrganization>>();
        collection.SetupCollection(userOrganization);

        // Act
        var mongoStateContainer = new MongoStateContainer<UserOrganization>(collection.Object);
        await mongoStateContainer.DeleteAsync(userOrganization.Id);

        // Assert
        collection.Verify(c => c.DeleteOneAsync(It.IsAny<FilterDefinition<UserOrganization>>(), CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public void Distinct_Ok()
    {
        // Arrange
        var userOrganization1 = new UserOrganization
        {
            Id = Identifier.GenerateString()
        };

        var userOrganization2 = new UserOrganization
        {
            Id = Identifier.GenerateString()
        };

        var collection = new Mock<IMongoCollection<UserOrganization>>();
        collection.SetupCollection(userOrganization1, userOrganization2);

        // Act
        var mongoStateContainer = new MongoStateContainer<UserOrganization>(collection.Object);
        var result = mongoStateContainer.Distinct<string>("Id");

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task FetchAsync_Ok()
    {
        // Arrange
        var userOrganization1 = new UserOrganization
        {
            Id = Identifier.GenerateString()
        };

        var userOrganization2 = new UserOrganization
        {
            Id = Identifier.GenerateString()
        };

        var collection = new Mock<IMongoCollection<UserOrganization>>();
        collection.SetupCollection(userOrganization1, userOrganization2);

        // Act
        var mongoStateContainer = new MongoStateContainer<UserOrganization>(collection.Object);
        var result = await mongoStateContainer.FetchAsync();

        // Assert
        var userOrganizations = result as UserOrganization[] ?? result.ToArray();
        userOrganizations.Should().NotBeNull();
        userOrganizations.Should().BeEquivalentTo(new[] { userOrganization1, userOrganization2 });
    }

    [Fact]
    public async Task FetchAsync_WithAllFilters()
    {
        // Arrange
        var userOrganization1 = new UserOrganization
        {
            Id = Identifier.GenerateString()
        };

        var userOrganization2 = new UserOrganization
        {
            Id = Identifier.GenerateString()
        };

        var collection = new Mock<IMongoCollection<UserOrganization>>();
        collection.SetupCollection(userOrganization1, userOrganization2);

        // Act
        var mongoStateContainer = new MongoStateContainer<UserOrganization>(collection.Object);
        var result = await mongoStateContainer.FetchAsync(
            new Filter(nameof(UserOrganization.Id), userOrganization1.Id),
            new Filter(nameof(UserOrganization.Id), userOrganization2.Id, FilterOperator.NotEqual),
            new Filter(nameof(UserOrganization.Id), userOrganization1.Id, FilterOperator.GreaterThan),
            new Filter(nameof(UserOrganization.Id), userOrganization2.Id, FilterOperator.GreaterThanOrEqual),
            new Filter(nameof(UserOrganization.Id), userOrganization1.Id, FilterOperator.LessThan),
            new Filter(nameof(UserOrganization.Id), userOrganization2.Id, FilterOperator.LessThanOrEqual),
            new Filter(nameof(UserOrganization.Id), userOrganization1.Id, FilterOperator.Regex),
            new Filter(nameof(UserOrganization.Id), DateTime.Now.ToString(CultureInfo.InvariantCulture))
        );

        // Assert
        var userOrganizations = result as UserOrganization[] ?? result.ToArray();
        userOrganizations.Should().NotBeNull();
        userOrganizations.Should().BeEquivalentTo(new[] { userOrganization1, userOrganization2 });
    }

    [Fact]
    public async Task FetchAsync_WithFilters()
    {
        // Arrange
        var userOrganization1 = new UserOrganization
        {
            Id = Identifier.GenerateString()
        };

        var userOrganization2 = new UserOrganization
        {
            Id = Identifier.GenerateString()
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
        userOrganizations.Should().BeEquivalentTo(new[] { userOrganization1, userOrganization2 });
    }

    [Fact]
    public async Task FetchAsync_WithFiltersUnknown()
    {
        // Arrange
        var userOrganization1 = new UserOrganization
        {
            Id = Identifier.GenerateString()
        };

        var userOrganization2 = new UserOrganization
        {
            Id = Identifier.GenerateString()
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
    public async Task ListAsync_Ok()
    {
        // Arrange
        var userOrganization1 = new UserOrganization
        {
            Id = Identifier.GenerateString()
        };

        var userOrganization2 = new UserOrganization
        {
            Id = Identifier.GenerateString()
        };

        var collection = new Mock<IMongoCollection<UserOrganization>>();
        collection.SetupCollection(userOrganization1, userOrganization2);

        // Act
        var mongoStateContainer = new MongoStateContainer<UserOrganization>(collection.Object);
        var result = await mongoStateContainer.ListAsync();


        // Assert
        var organizations = result.ToList();
        organizations.Should().NotBeNull();
        organizations.Should().BeEquivalentTo(new[] { userOrganization1, userOrganization2 });
    }

    [Fact]
    public async Task ReadAsync_Ok()
    {
        // Arrange
        var userOrganization = new UserOrganization
        {
            Id = Identifier.GenerateString()
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
    public async Task SearchAsync_Ok()
    {
        // Arrange
        var userOrganization1 = new UserOrganization
        {
            Id = Identifier.GenerateString()
        };

        var userOrganization2 = new UserOrganization
        {
            Id = Identifier.GenerateString()
        };

        var collection = new Mock<IMongoCollection<UserOrganization>>();
        collection.SetupCollection(userOrganization1, userOrganization2);

        // Act
        var mongoStateContainer = new MongoStateContainer<UserOrganization>(collection.Object);
        var result = await mongoStateContainer.SearchAsync(
            new Filter(nameof(UserOrganization.Id), userOrganization1.Id)
        );

        // Assert
        var userOrganizations = result as UserOrganization[] ?? result.ToArray();
        userOrganizations.Should().NotBeNull();
        userOrganizations.Should().BeEquivalentTo(new[] { userOrganization1, userOrganization2 });
    }

    [Fact]
    public async Task WriteAsync_Ok()
    {
        // Arrange
        var userOrganization = new UserOrganization
        {
            Id = Identifier.GenerateString()
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
}