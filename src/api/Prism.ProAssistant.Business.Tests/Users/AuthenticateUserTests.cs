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
using Prism.ProAssistant.Business.Tests;
using Prism.ProAssistant.Business.Users;

namespace Prism.ProAssistant.Tests.Users;

public class AuthenticateUserTests
{
    private readonly JwtConfiguration _jwtConfiguration = new()
    {
        PrivateKey =
            "MIICXAIBAAKBgQC7mfR1xgcrRJ+ceZlWpbbXy9dllF9FUXgQU4SuJfby6nXskt2tNX4AOF0dqm/1E1pwy79RhetS/Cj6slnasfwhCI9cwDYVSM1SYsYxr5WXR+KmPUQGC2Upcxt1pB5qj9ow5qq/zOIJ3xwuD+q6VVlAqvF1vKClX/DINToa1I/XbQIDAQABAoGAJdEcGV2o9kzoC+frRC81k3yw1/Y32kZY+JmNZnmatU8UJHNaol7lHnA+PQutc+7JzXEVCP8A+AKC1D59pHs6gql24dCD1Ca9FfS5qTd4SIawUYqcL0OPSVkJPhL+/wVJVOLQG7ALiOhmyNZADSKCj/fKb3bQZH57Sj5osPqGZ4ECQQDwXyBhYwKXZTP4uYTpWX5or/NGF9t1vNdo5tzkspvOFmW81bbGoVg8wzshVmT9cLKlVFHMls1pDWqhCboGHevhAkEAx8x+MGb4haCwQVMnLmthCQmnB7n5uADv2KoVA0zXc7k/reRVgW7aSj4bKNnBrHqdhG8ELDCc02leSESe1J59DQJAevP9yTLvGWgADKNA9Gf9vCj8ZIdBj9kXyqYEqcse3W0hf1VGWBYh33rx3RynLeieyOj3qpIc4jalq1ghWo2loQJBALtMo5tCXIYAjlqe1iM4/H1ZdCDVIhlxn2awgwRV+7/7kIu2esXconxo3lMcV+gWBiZJYFMAu3Og2obK9U6CyN0CQDyfKjEQ4mveAMeYlnDTJmc2CaOVQ2je8SJd5dCwUZbF3nZPgWeuJ/R8jzQy+qxN3ICxRa+D1+ATNm0rmfnb25k=",
        PublicKey =
            "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC7mfR1xgcrRJ+ceZlWpbbXy9dllF9FUXgQU4SuJfby6nXskt2tNX4AOF0dqm/1E1pwy79RhetS/Cj6slnasfwhCI9cwDYVSM1SYsYxr5WXR+KmPUQGC2Upcxt1pB5qj9ow5qq/zOIJ3xwuD+q6VVlAqvF1vKClX/DINToa1I/XbQIDAQAB"
    };

    [Fact]
    public async Task Handle_Admin_Ko_WhenOthers()
    {
        // Arrange
        var otherUser = new User(Identifier.GenerateString(), Identifier.GenerateString(), Identifier.GenerateString(), Identifier.GenerateString());
        var request = new AuthenticateUser("admin", "admin");
        var database = new Mock<IMongoDatabase>();
        database.SetupCollection(otherUser);

        // Act
        var handler = new AuthenticateUserHandler(Mock.Of<ILogger<AuthenticateUserHandler>>(), database.Object, _jwtConfiguration);
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Result.Should().Be(HttpStatusCode.NotFound);
        result.AccessToken.Should().BeNull();
    }

    [Fact]
    public void Validate_Ok()
    {
        // Arrange
        var request = new AuthenticateUser(Identifier.GenerateString(), Identifier.GenerateString());
        
        // Act
        var validator = new AuthenticateUserValidator();
        var result = validator.Validate(request);

        // Assert
    }

    [Fact]
    public async Task Handle_Admin_Ok()
    {
        // Arrange
        var request = new AuthenticateUser("admin", "admin");
        var database = new Mock<IMongoDatabase>();
        database.SetupCollection<User>();

        // Act
        var handler = new AuthenticateUserHandler(Mock.Of<ILogger<AuthenticateUserHandler>>(), database.Object, _jwtConfiguration);
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Result.Should().Be(HttpStatusCode.OK);
        result.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Handle_Admin_Ok_WithPassword()
    {
        // Arrange
        var password = Identifier.GenerateString();
        var request = new AuthenticateUser("admin", password);
        var user = new User(Identifier.GenerateString(), "admin", Argon2.Hash(password, 1, 42), "Admin");
        var database = new Mock<IMongoDatabase>();
        database.SetupCollection(user);

        // Act
        var handler = new AuthenticateUserHandler(Mock.Of<ILogger<AuthenticateUserHandler>>(), database.Object, _jwtConfiguration);
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Result.Should().Be(HttpStatusCode.OK);
        result.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Handle_User_NotFOund()
    {
        // Arrange
        var password = Identifier.GenerateString();
        var request = new AuthenticateUser("simon", password);
        var user = new User(Identifier.GenerateString(), "admin", Argon2.Hash(password, 1, 42), "Admin");
        var database = new Mock<IMongoDatabase>();
        database.SetupCollectionFindEmpty(user);

        // Act
        var handler = new AuthenticateUserHandler(Mock.Of<ILogger<AuthenticateUserHandler>>(), database.Object, _jwtConfiguration);
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Result.Should().Be(HttpStatusCode.NotFound);
        result.AccessToken.Should().BeNullOrWhiteSpace();
    }
}