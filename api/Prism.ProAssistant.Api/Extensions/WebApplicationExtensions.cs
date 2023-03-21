using Prism.ProAssistant.Api.Exceptions;

namespace Prism.ProAssistant.Api.Extensions;

public static class WebApplicationExtensions
{
    public static void ReplaceEnvironmentVariables(this WebApplication app)
    {
        app.Logger.LogInformation("Replacing environment variables in JavaScript files...");

        foreach (var file in Directory.GetFiles(app.Environment.WebRootPath, "*.js", SearchOption.AllDirectories))
        {
            app.Logger.LogInformation("Replacing environment variables in {file}...", file);
            var content = File.ReadAllText(file);

            content = ReplaceEnvironmentVariable(content, app.Logger, "ENV_AZURE_AD_CLIENT_ID", Environment.GetEnvironmentVariable("AZURE_AD_CLIENT_ID"));
            content = ReplaceEnvironmentVariable(content, app.Logger, "ENV_AZURE_AD_TENANT_ID", Environment.GetEnvironmentVariable("AZURE_AD_TENANT_ID"));
            content = ReplaceEnvironmentVariable(content, app.Logger, "ENV_AZURE_AD_USER_FLOW", Environment.GetEnvironmentVariable("AZURE_AD_USER_FLOW"));
            content = ReplaceEnvironmentVariable(content, app.Logger, "ENV_AZURE_AD_TENANT_NAME", Environment.GetEnvironmentVariable("AZURE_AD_TENANT_NAME"));

            content = content.Replace("ENV_APPLICATIONINSIGHTS_CONNECTION_STRING", Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING"));

            File.WriteAllText(file, content);
        }
    }

    private static string ReplaceEnvironmentVariable(string content, ILogger logger, string variable, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new NotFoundException($"The environment variable {variable} was not found.");
        }

        var size = content.Length;
        content = content.Replace(variable, value);

        if (content.Length != size)
        {
            logger.LogInformation("The environment variable {variable} was replaced in the JavaScript file", variable);
        }

        return content;
    }
}