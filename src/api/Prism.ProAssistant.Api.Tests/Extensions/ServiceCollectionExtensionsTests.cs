// -----------------------------------------------------------------------
//  <copyright file = "ServiceCollectionExtensionsTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Prism.ProAssistant.Api.Extensions;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Extensions
{
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
            services.AddBusinessServices();

            // Assert
            services.Should().Contain(x => x.ServiceType == typeof(IUserContextAccessor));
        }

        [Fact]
        public void AddCache()
        {
            // Arrange
            Environment.SetEnvironmentVariable("REDIS_CONNECTION_STRING", "localhost:6379");
            Environment.SetEnvironmentVariable("ENVIRONMENT", "UNIT TESTS");
            var services = new ServiceCollection();

            // Act
            services.AddCache();

            // Assert
            services.Should().Contain(x => x.ServiceType == typeof(IDistributedCache));
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
            services.Should().Contain(x => x.ServiceType == typeof(IOrganizationContext));
        }

        [Fact]
        public void AddQueriesCommands()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddQueriesCommands();

            // Assert
            services.Should().Contain(x => x.ServiceType == typeof(IMediator));
        }
    }
}