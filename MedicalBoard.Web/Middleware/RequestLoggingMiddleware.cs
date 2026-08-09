using System.Diagnostics;

namespace MedicalBoard.Web.Middleware;

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
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var username = context.User.Identity?.IsAuthenticated == true ? context.User.Identity!.Name : "anonymous";

            _logger.LogInformation(
                "{Method} {Path} by {Username} responded {StatusCode} in {Elapsed}ms",
                context.Request.Method,
                context.Request.Path,
                username,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}
