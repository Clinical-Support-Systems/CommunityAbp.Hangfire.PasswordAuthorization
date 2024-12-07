using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Volo.Abp;
using Volo.Abp.Hangfire;
using Volo.Abp.Modularity;
using Volo.Abp.DependencyInjection;

namespace CommunityAbp.Hangfire.PasswordAuthorization
{
    [DependsOn(typeof(AbpHangfireModule))]
    public class HangfirePasswordAuthorizationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;

            services.AddScoped<IDashboardAuthorizationFilter, HangfirePasswordAuthorizationFilter>();
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.ServiceProvider.GetRequiredService<IObjectAccessor<IApplicationBuilder>>().Value;
            var env = context.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            // Register the authentication middleware
            app.UseMiddleware<HangfireAuthenticationMiddleware>();
        }
    }

    public class HangfireAuthorizationOptions
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class FlexibleHangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly FlexibleHangfireAuthOptions _options;

        public FlexibleHangfireAuthorizationFilter(IOptions<FlexibleHangfireAuthOptions> options)
        {
            _options = options.Value;
        }

        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Check if user is already authenticated via ABP (cookie-based)
            if (httpContext.User?.Identity?.IsAuthenticated == true)
            {
                // If a role is specified and the user must have it
                if (!string.IsNullOrEmpty(_options.RequiredRole))
                {
                    if (httpContext.User.IsInRole(_options.RequiredRole))
                    {
                        return true; // Has the required role
                    }
                }

                // If specific users are allowed
                if (_options.AllowedUsernames?.Any() == true)
                {
                    var username = httpContext.User.Identity.Name;
                    if (_options.AllowedUsernames.Contains(username, StringComparer.OrdinalIgnoreCase))
                    {
                        return true; // Is an explicitly allowed user
                    }
                }

                // If no specific conditions were given or user doesn't match them, fall through to next check
            }

            // If not authenticated via ABP or didn't meet ABP-based conditions, try Basic Auth check
            var authHeader = httpContext.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrWhiteSpace(_options.BasicAuthUsername) &&
                !string.IsNullOrWhiteSpace(_options.BasicAuthPassword) &&
                authHeader.StartsWith("Basic "))
            {
                // Validate Basic Auth credentials
                var encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
                var credentials = System.Text.Encoding.UTF8.GetString(
                    Convert.FromBase64String(encodedCredentials)).Split(':');

                if (credentials.Length == 2)
                {
                    var user = credentials[0];
                    var pass = credentials[1];

                    if (user.Equals(_options.BasicAuthUsername, StringComparison.OrdinalIgnoreCase) &&
                        pass == _options.BasicAuthPassword)
                    {
                        return true; // Basic Auth success
                    }
                }
            }

            // If we reach here, none of the methods worked
            return false;
        }
    }

    public class FlexibleHangfireAuthOptions
    {
        // Role that ABP-authenticated user must have (optional)
        public string? RequiredRole { get; set; }

        // A list of ABP usernames allowed to access (optional)
        public List<string>? AllowedUsernames { get; set; }

        // Basic auth fallback credentials (optional)
        public string? BasicAuthUsername { get; set; }
        public string? BasicAuthPassword { get; set; }
    }

    public class HangfirePasswordAuthorizationFilter : AuthorizeAttribute, IDashboardAuthorizationFilter
    {
        [Authorize]
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            if (httpContext.User.Identity?.IsAuthenticated ?? false)
            {
                return true;
            }

            httpContext.Response.StatusCode = 301;
            httpContext.Response.Redirect(DashboardAuthorizationRouteConfig.LoginPath);
            httpContext.Response.WriteAsync("ok");
            return false;
        }
    }

    internal static class DashboardAuthorizationRouteConfig
    {
        private const string AuthorizationPrefix = "/authorization";

        internal static string PathMatch { get; set; } = "/hangfire";

        internal static string LoginPath => $"{PathMatch}{AuthorizationPrefix}/login.html";
        internal static string AuthorizationPath => $"{PathMatch}{AuthorizationPrefix}";
        internal static string CaptchaPath => $"{PathMatch}{AuthorizationPrefix}/captcha";
    }

    public class HangfireAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HangfireAuthorizationOptions _options;

        public HangfireAuthenticationMiddleware(RequestDelegate next,
            IOptions<HangfireAuthorizationOptions> options)
        {
            _next = next;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (IsHangfirePath(context.Request.Path))
            {
                string authHeader = context.Request.Headers["Authorization"];

                if (authHeader != null && authHeader.StartsWith("Basic "))
                {
                    // Extract credentials
                    var encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
                    var credentials = System.Text.Encoding.UTF8.GetString(
                        Convert.FromBase64String(encodedCredentials)).Split(':');

                    if (credentials[0] == _options.Username &&
                        credentials[1] == _options.Password)
                    {
                        // Create claims identity
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, credentials[0]),
                            new Claim(ClaimTypes.Role, "HangfireUser")
                        };

                        var identity = new ClaimsIdentity(claims, "Basic");
                        context.User = new ClaimsPrincipal(identity);

                        await _next(context);
                        return;
                    }
                }

                // If no auth header or invalid credentials, return 401
                context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Hangfire Dashboard\"";
                context.Response.StatusCode = 401;
                return;
            }

            await _next(context);
        }

        private bool IsHangfirePath(PathString path)
        {
            return path.StartsWithSegments("/hangfire", StringComparison.OrdinalIgnoreCase);
        }
    }
}
