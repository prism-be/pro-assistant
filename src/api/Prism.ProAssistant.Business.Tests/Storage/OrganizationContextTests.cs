// -----------------------------------------------------------------------
//  <copyright file = "OrganizationContextTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Tests.Storage;

public class OrganizationContextTests
{
    [Fact]
    public void GetCollection_InvalidObject()
    {
        // Arrange
        var configuration = new MongoDbConfiguration("mongodb://proassistant:Toto123Toto123@localhost:27017/?authSource=admin");
        var user = new Mock<User>();

        // Act
        var context = new OrganizationContext(new MongoClient(configuration.ConnectionString), user.Object);

        // Assert
        Assert.Throws<NotSupportedException>(() => context.GetCollection<object>());
    }

    [Fact]
    public void GetCollection_Ok()
    {
        // Arrange
        var configuration = new MongoDbConfiguration("mongodb://proassistant:Toto123Toto123@localhost:27017/?authSource=admin");
        var user = new Mock<User>();

        // Act
        var context = new OrganizationContext(new MongoClient(configuration.ConnectionString), user.Object);
        var collection = context.GetCollection<Contact>();

        // Assert
        collection.Should().NotBeNull();
    }

    [Fact]
    public void GetCollection_Ok_KnownOrganization()
    {
        // Arrange
        var configuration = new MongoDbConfiguration("mongodb://proassistant:Toto123Toto123@localhost:27017/?authSource=admin");
        var user = new Mock<User>();
        user.Setup(x => x.Organization).Returns(Identifier.GenerateString);

        // Act
        var context = new OrganizationContext(new MongoClient(configuration.ConnectionString), user.Object);
        var collection = context.GetCollection<Contact>();

        // Assert
        collection.Should().NotBeNull();
    }
}