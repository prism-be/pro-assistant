using System.Security.Cryptography;

namespace Prism.ProAssistant.Business.Security;

public static class Identifier
{
    public static Guid Generate()
    {
        return new Guid(RandomNumberGenerator.GetBytes(16));
    }

    public static string GenerateString()
    {
        return Generate().ToString();
    }
}