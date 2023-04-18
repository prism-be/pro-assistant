using Prism.Core;
using Prism.Core.Attributes;

namespace Prism.Infrastructure.Authentication;

[Collection("users")]
[Partition(nameof(Id))]
public class UserOrganization
{
    public string Id { get; set; } = Guid.Empty.ToString();
    public string Organization { get; set; } = "demo";
}