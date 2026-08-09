bash

bash -c '
cd /home/claude/MedicalBoard

cat > Web/Extensions/MiddlewareExtensions.cs << "EOF"
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
EOF

cat > Web/Extensions/ServiceCollectionExtensions.cs << "EOF"
using MedicalBoard.Application.Authorization;
using MedicalBoard.Application.Interfaces;
using MedicalBoard.Application.Services;
using MedicalBoard.Domain.Entities;
using MedicalBoard.Infrastructure.Authentication;
using MedicalBoard.Infrastructure.Authorization;
using MedicalBoard.Infrastructure.Data;
using MedicalBoard.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MedicalBoard.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMedicalBoardPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // DIP: application services depend on IApplicationDbContext, not the concrete DbContext.
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    public static IServiceCollection AddMedicalBoardApplicationServices(this IServiceCollection services)
    {
        // New in this milestone set.
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IReportService, ReportService>();

        // NOTE: keep your existing registrations for IDepartmentService, IDoctorService,
        // IPatientService and IAppointmentService from Milestone 2 alongside these.

        services.AddScoped<IUserPermissionProvider, EfUserPermissionProvider>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

        return services;
    }

    public static IServiceCollection AddMedicalBoardAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.LogoutPath = "/Account/Logout";
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
            })
            // Milestone 10 -- custom scheme, only used where explicitly requested via
            // [Authorize(AuthenticationSchemes = ApiKeyAuthenticationSchemeOptions.DefaultScheme)].
            .AddScheme<ApiKeyAuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
                ApiKeyAuthenticationSchemeOptions.DefaultScheme, _ => { });

        return services;
    }

    public static IServiceCollection AddMedicalBoardAuthorization(this IServiceCollection services)
    {
        // Dynamic permission-code policies (Milestone 8).
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        // Resource-based ownership check (Milestone 11). Use from a controller like:
        //   var authResult = await _authorizationService.AuthorizeAsync(User, appointment, new AppointmentOwnershipRequirement());
        services.AddScoped<IAuthorizationHandler, AppointmentOwnershipHandler>();

        services.AddAuthorization(options =>
        {
            // Stage 2 -- coarse role policies, still useful alongside the fine-grained permission policies.
            options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
            options.AddPolicy("RequireManagerOrAdmin", policy => policy.RequireRole("Admin", "Manager"));
        });

        return services;
    }
}
EOF
echo "extensions ok"
'
Output

extensions ok
Done


