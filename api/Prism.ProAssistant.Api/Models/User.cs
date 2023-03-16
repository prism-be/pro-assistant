namespace Prism.ProAssistant.Api.Models;

public class User
{

    public bool IsAuthenticated { get; set; }
    public string? Id { get; set; }

    public string? Name { get; set; }

    public string? Organization { get; set; }
}