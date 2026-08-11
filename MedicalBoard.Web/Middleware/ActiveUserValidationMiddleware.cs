using System.Security.Claims;
using MedicalBoard.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace MedicalBoard.Web.Middleware;

// If an authenticated user's account was deactivated after the cookie was issued,
// this middleware signs them out on the next request instead of trusting the stale claim.
public class ActiveUserValidationMiddleware
{
    private readonly RequestDelegate _next;
    public ActiveUserValidationMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdClaim, out var userId))
            {
                var isActive = await dbContext.Users
                    .AsNoTracking()
                    .Where(u => u.Id == userId)
                    .Select(u => (bool?)u.IsActive)
                    .FirstOrDefaultAsync();

                if (isActive != true)
                {
                    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    context.Response.Redirect("/Account/Login?deactivated=1");
                    return;
                }
            }
        }

        await _next(context);
    }
}
