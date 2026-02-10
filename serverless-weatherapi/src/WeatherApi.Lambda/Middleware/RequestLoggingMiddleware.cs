namespace WeatherApi.Lambda.Middleware;

public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var start = DateTime.UtcNow;
        _logger.LogInformation("HTTP {Method} {Path}", context.Request.Method, context.Request.Path);

        await _next(context);

        var elapsedMs = (DateTime.UtcNow - start).TotalMilliseconds;
        _logger.LogInformation("HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            elapsedMs);
    }
}
