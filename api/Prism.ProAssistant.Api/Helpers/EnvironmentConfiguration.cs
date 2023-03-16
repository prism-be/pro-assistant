using Prism.ProAssistant.Api.Exceptions;

namespace Prism.ProAssistant.Business;

public static class EnvironmentConfiguration
{
    public static string? GetConfiguration(string configurationKey, bool required = false)
    {
        var value = Environment.GetEnvironmentVariable(configurationKey);

        if (string.IsNullOrWhiteSpace(value) && required)
        {
            throw new MissingConfigurationException($"The configuration {configurationKey} is required", configurationKey);
        }

        return value;
    }

    public static string GetMandatoryConfiguration(string configurationKey)
    {
        var value = GetConfiguration(configurationKey, true);
        return value!;
    }
}