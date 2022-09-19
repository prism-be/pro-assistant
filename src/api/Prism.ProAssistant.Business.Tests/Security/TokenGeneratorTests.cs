using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.ProAssistant.Business.Exceptions;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Users;

namespace Prism.ProAssistant.Tests.Security;

public class TokenGeneratorTests
{
    private readonly JwtConfiguration _jwtConfiguration = new()
    {
        PrivateKey =
            "MIICXAIBAAKBgQC7mfR1xgcrRJ+ceZlWpbbXy9dllF9FUXgQU4SuJfby6nXskt2tNX4AOF0dqm/1E1pwy79RhetS/Cj6slnasfwhCI9cwDYVSM1SYsYxr5WXR+KmPUQGC2Upcxt1pB5qj9ow5qq/zOIJ3xwuD+q6VVlAqvF1vKClX/DINToa1I/XbQIDAQABAoGAJdEcGV2o9kzoC+frRC81k3yw1/Y32kZY+JmNZnmatU8UJHNaol7lHnA+PQutc+7JzXEVCP8A+AKC1D59pHs6gql24dCD1Ca9FfS5qTd4SIawUYqcL0OPSVkJPhL+/wVJVOLQG7ALiOhmyNZADSKCj/fKb3bQZH57Sj5osPqGZ4ECQQDwXyBhYwKXZTP4uYTpWX5or/NGF9t1vNdo5tzkspvOFmW81bbGoVg8wzshVmT9cLKlVFHMls1pDWqhCboGHevhAkEAx8x+MGb4haCwQVMnLmthCQmnB7n5uADv2KoVA0zXc7k/reRVgW7aSj4bKNnBrHqdhG8ELDCc02leSESe1J59DQJAevP9yTLvGWgADKNA9Gf9vCj8ZIdBj9kXyqYEqcse3W0hf1VGWBYh33rx3RynLeieyOj3qpIc4jalq1ghWo2loQJBALtMo5tCXIYAjlqe1iM4/H1ZdCDVIhlxn2awgwRV+7/7kIu2esXconxo3lMcV+gWBiZJYFMAu3Og2obK9U6CyN0CQDyfKjEQ4mveAMeYlnDTJmc2CaOVQ2je8SJd5dCwUZbF3nZPgWeuJ/R8jzQy+qxN3ICxRa+D1+ATNm0rmfnb25k=",
        PublicKey =
            "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC7mfR1xgcrRJ+ceZlWpbbXy9dllF9FUXgQU4SuJfby6nXskt2tNX4AOF0dqm/1E1pwy79RhetS/Cj6slnasfwhCI9cwDYVSM1SYsYxr5WXR+KmPUQGC2Upcxt1pB5qj9ow5qq/zOIJ3xwuD+q6VVlAqvF1vKClX/DINToa1I/XbQIDAQAB"
    };

    [Fact]
    public void GenerateAccessToken_MissingPublicKey()
    {
        // Arrange
        var user = new User(Identifier.GenerateString(), Identifier.GenerateString(), Identifier.GenerateString(), Identifier.GenerateString());

        // Act
        var token = TokenGenerator.GenerateAccessToken(_jwtConfiguration.PrivateKey, user);
        Assert.Throws<MissingConfigurationException>(() => TokenGenerator.ValidateToken(string.Empty, token, Mock.Of<ILogger>(), false));
    }

    [Fact]
    public void GenerateAccessToken_Ok()
    {
        // Arrange
        var user = new User(Identifier.GenerateString(), Identifier.GenerateString(), Identifier.GenerateString(), Identifier.GenerateString());

        // Act
        var token = TokenGenerator.GenerateAccessToken(_jwtConfiguration.PrivateKey, user);
        var claims = TokenGenerator.ValidateToken(_jwtConfiguration.PublicKey, token, Mock.Of<ILogger>(), false);

        var claimsUserId = claims?.FindFirst(ClaimsNames.UserId)?.Value ?? Guid.Empty.ToString();

        // Assert
        claimsUserId.Should().Be(user.Id);
    }

    [Fact]
    public void GenerateRefreshToken_Ok()
    {
        // Arrange
        var user = new User(Identifier.GenerateString(), Identifier.GenerateString(), Identifier.GenerateString(), Identifier.GenerateString());

        // Act
        var token = TokenGenerator.GenerateRefreshToken(_jwtConfiguration.PrivateKey, user);
        var claims = TokenGenerator.ValidateToken(_jwtConfiguration.PublicKey, token, Mock.Of<ILogger>(), true);

        var claimsUserId = claims?.FindFirst(ClaimsNames.UserId)?.Value ?? Guid.Empty.ToString();

        // Assert
        claimsUserId.Should().Be(user.Id);
    }
}