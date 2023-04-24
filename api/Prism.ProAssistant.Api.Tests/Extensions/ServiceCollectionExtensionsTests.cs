using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Prism.ProAssistant.Api.Config;
using Prism.ProAssistant.Api.Extensions;
using Prism.ProAssistant.Api.Services;
using Prism.ProAssistant.Storage;

namespace Prism.ProAssistant.Api.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{

    [Fact]
    public void AddBearer()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddBearer();

        // Assert
        services.Should().Contain(x => x.ServiceType == typeof(JwtBearerHandler));
    }

    [Fact]
    public void AddBusinessServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddProAssistant();

        // Assert
        services.Should().Contain(x => x.ServiceType == typeof(IQueryService));
    }

    [Fact]
    public void AddDatabase()
    {
        // Arrange
        Environment.SetEnvironmentVariable("MONGODB_CONNECTION_STRING", "mongodb://proassistant:Toto123Toto123@localhost:27017/?authSource=admin");
        var services = new ServiceCollection();

        // Act
        services.AddDatabase();

        // Assert
        services.Should().Contain(x => x.ServiceType == typeof(MongoDbConfiguration));
    }
}