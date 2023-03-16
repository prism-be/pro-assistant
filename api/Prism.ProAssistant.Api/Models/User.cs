namespace Prism.ProAssistant.Api.Models;

public class User
{
    public string? Id { get; set; }
    
    public string? Organization { get; set; }
    
    public string? Name { get; set; }
    
    public bool IsAuthenticated { get; set; }
}