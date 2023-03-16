namespace Prism.ProAssistant.Api.Middlewares;

public class ErrorLoggingMiddleware
{
    private readonly ILogger<ErrorLoggingMiddleware> _logger;
    private readonly RequestDelegate _next;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ErrorLoggingMiddleware" /> class.
    /// </summary>
    /// <param name="next">The request delegate to call to pass the next middleware.</param>
    /// <param name="logger">The current logger.</param>
    public ErrorLoggingMiddleware(RequestDelegate next, ILogger<ErrorLoggingMiddleware> logger)
    {
        _logger = logger;
        _next = next;
    }

    /// <summary>
    ///     Invoke the middleware.
    /// </summary>
    /// <param name="httpContext">The current http context.</param>
    /// <returns>The task to be waited.</returns>
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while processing the request : {message}", ex.Message);
            throw;
        }
    }
}