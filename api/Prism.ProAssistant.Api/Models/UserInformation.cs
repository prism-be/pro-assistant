using System.Text.Json.Serialization;

namespace Prism.ProAssistant.Api.Models;

public class UserInformation
{
    public bool IsAuthenticated { get; set; }
    required public string Name { get; set; }

    required public string Organization { get; set; }
}