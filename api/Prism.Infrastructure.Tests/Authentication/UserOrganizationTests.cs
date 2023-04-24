using FluentAssertions;
using Prism.Infrastructure.Authentication;

namespace Prism.Infrastructure.Tests.Authentication;

public class UserOrganizationTests
{
    [Fact]
    public void Constructor_Ok()
    {
        // Act
        var userOrganization = new UserOrganization();

        // Assert
        userOrganization.Id.Should().Be(Guid.Empty.ToString());
        userOrganization.Organization.Should().Be("demo");
    }
}