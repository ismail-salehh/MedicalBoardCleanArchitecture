using MedicalBoard.Web.Middleware;

namespace MedicalBoard.Web.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<GlobalExceptionMiddleware>();

    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        => app.UseMiddleware<RequestLoggingMiddleware>();

    public static IApplicationBuilder UseActiveUserValidation(this IApplicationBuilder app)
        => app.UseMiddleware<ActiveUserValidationMiddleware>();
}
