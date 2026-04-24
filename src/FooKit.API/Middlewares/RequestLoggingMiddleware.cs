using System.Diagnostics;

namespace MyProject.API.Middlewares;

public class RequestLoggingMiddleware
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
        var sw = Stopwatch.StartNew();
        
        await _next(context);
        
        sw.Stop();
        
        var statusCode = context.Response.StatusCode;
        var method = context.Request.Method;
        var path = context.Request.Path;
        var userId = context.User.Identity?.IsAuthenticated == true ? context.User.Identity.Name : "Anonymous";

        _logger.LogInformation("HTTP {Method} {Path} responded {StatusCode} in {Elapsed}ms (User: {UserId})",
            method, path, statusCode, sw.ElapsedMilliseconds, userId);
    }
}
