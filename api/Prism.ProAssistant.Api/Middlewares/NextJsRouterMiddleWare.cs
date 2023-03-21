namespace Prism.ProAssistant.Api.Middlewares;

public class NextJsRouterMiddleWare
{
    private readonly RequestDelegate _next;
    private readonly ILogger<NextJsRouterMiddleWare> _logger;

    public NextJsRouterMiddleWare(RequestDelegate next, ILogger<NextJsRouterMiddleWare> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path.Value;

        if (path == null || path.StartsWith("/api"))
        {
            await _next(context);
            return;
        }
        
        // Simple case: corresponding file exists
        var file = $"wwwroot{path}.html";
        if (File.Exists(file))
        {
            _logger.LogInformation("Serving file: {file} for path {path}", file, path);
            await context.Response.SendFileAsync(file);
            return;
        }
        
        // Complex case: a catch-all route exists
        var directory = $"wwwroot{path.Substring(0, path.LastIndexOf("/", StringComparison.Ordinal))}/";
        var catchAll = Directory.GetFiles(directory, "[*].html").FirstOrDefault();
        if (catchAll != null)
        {
            _logger.LogInformation("Serving file: {file} for path {path}", catchAll, path);
            await context.Response.SendFileAsync(catchAll);
            return;
        }

        await _next(context);
    }
}