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
using Volo.Abp.BackgroundJobs.Hangfire;

namespace CommunityAbp.Hangfire.PasswordAuthorization
{
    /// <summary>
    /// Module for adding password authorization to Hangfire dashboard.
    /// </summary>
    [DependsOn(typeof(AbpHangfireModule), typeof(AbpBackgroundJobsHangfireModule))]
    public class CommunityAbpHangfirePasswordAuthorizationModule : AbpModule
    {
        /// <inheritdoc/>
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;

            services.AddScoped<IDashboardAuthorizationFilter, HangfirePasswordAuthorizationFilterAttribute>();
        }

        /// <inheritdoc/>
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.ServiceProvider.GetRequiredService<IObjectAccessor<IApplicationBuilder>>().Value;

            // Register the authentication middleware
            (app ?? throw new InvalidOperationException()).UseMiddleware<HangfireAuthenticationMiddleware>();
        }
    }

    /// <summary>
    /// Extension methods for configuring Hangfire dashboard with password authorization.
    /// </summary>
    public class FlexibleHangfireAuthOptions
    {
        /// <summary>
        /// A list of ABP usernames allowed to access (optional)
        /// </summary>
        public List<string>? AllowedUsernames { get; set; }

        /// <summary>
        /// The basic auth password 
        /// </summary>
        public string? BasicAuthPassword { get; set; }

        /// <summary>
        /// Basic auth fallback credentials (optional)
        /// </summary>
        public string? BasicAuthUsername { get; set; }

        /// <summary>
        /// Role that ABP-authenticated user must have (optional)
        /// </summary>
        public string? RequiredRole { get; set; }
    }

    /// <summary>
    /// Hangfire flexible authorization filter 
    /// </summary>
    public class FlexibleHangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly FlexibleHangfireAuthOptions _options;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options">The options for Hangfire authorization</param>
        public FlexibleHangfireAuthorizationFilter(IOptions<FlexibleHangfireAuthOptions> options)
        {
            _options = options.Value;
        }

        /// <summary>
        /// Authorize the Hangfire dashboard access.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Check if user is already authenticated via ABP (cookie-based)
            if (httpContext.User?.Identity?.IsAuthenticated == true)
            {
                // If a role is specified and the user must have it
                if (!string.IsNullOrEmpty(_options.RequiredRole) && httpContext.User.IsInRole(_options.RequiredRole))
                {
                    return true; // Has the required role
                }

                // If specific users are allowed
                if (_options.AllowedUsernames?.Count > 0)
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
            var authHeader = httpContext.Request.Headers.Authorization.ToString();
            if (string.IsNullOrWhiteSpace(_options.BasicAuthUsername) ||
                string.IsNullOrWhiteSpace(_options.BasicAuthPassword) ||
                !authHeader.StartsWith("Basic ")) return false;

            // Validate Basic Auth credentials
            var encodedCredentials = authHeader["Basic ".Length..].Trim();
            var credentials = System.Text.Encoding.UTF8.GetString(
                Convert.FromBase64String(encodedCredentials)).Split(':');

            if (credentials.Length != 2) return false;
            var user = credentials[0];
            var pass = credentials[1];

            return user.Equals(_options.BasicAuthUsername, StringComparison.OrdinalIgnoreCase) &&
                   pass == _options.BasicAuthPassword;
        }
    }

    /// <summary>
    ///  The hangfire dashboard authentication middleware 
    /// </summary>
    public class HangfireAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HangfireAuthorizationOptions _options;

        /// <summary>
        /// The hangfire dashboard authentication middleware 
        /// </summary>
        /// <param name="next">
        /// The next middleware in the pipeline
        /// </param>
        /// <param name="options">
        /// The options for hangfire dashboard authorization
        /// </param>
        public HangfireAuthenticationMiddleware(RequestDelegate next,
            IOptions<HangfireAuthorizationOptions> options)
        {
            _next = next;
            _options = options.Value;
        }

        /// <summary>
        /// Middleware for Hangfire dashboard authorization.
        /// </summary>
        /// <param name="context"></param>
        public async Task InvokeAsync(HttpContext context)
        {
            if (IsHangfirePath(context.Request.Path))
            {
                var authHeader = context.Request.Headers.Authorization.ToString();

                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Basic "))
                {
                    // Extract credentials
                    var encodedCredentials = authHeader["Basic ".Length..].Trim();
                    var credentials = System.Text.Encoding.UTF8.GetString(
                        Convert.FromBase64String(encodedCredentials)).Split(':');

                    if (credentials[0] == _options.Username &&
                        credentials[1] == _options.Password)
                    {
                        // Create claims identity
                        var claims = new List<Claim>
                        {
                            new(ClaimTypes.Name, credentials[0]),
                            new(ClaimTypes.Role, "HangfireUser")
                        };

                        var identity = new ClaimsIdentity(claims, "Basic");
                        context.User = new ClaimsPrincipal(identity);

                        await _next(context);
                        return;
                    }
                }

                // If no auth header or invalid credentials, return 401
                context.Response.Headers.WWWAuthenticate = "Basic realm=\"Hangfire Dashboard\"";
                context.Response.StatusCode = 401;
                return;
            }

            await _next(context);
        }

        private static bool IsHangfirePath(PathString path)
        {
            return path.StartsWithSegments("/hangfire", StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// Extension methods for configuring Hangfire dashboard with password authorization.
    /// </summary>
    public class HangfireAuthorizationOptions
    {
        /// <summary>
        /// The password
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// The username
        /// </summary>
        public string Username { get; set; } = string.Empty;
    }

    /// <summary>
    /// Filter for Hangfire dashboard authorization.
    /// </summary>
    public class HangfirePasswordAuthorizationFilterAttribute : AuthorizeAttribute, IDashboardAuthorizationFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
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

        internal static string AuthorizationPath => $"{PathMatch}{AuthorizationPrefix}";
        internal static string CaptchaPath => $"{PathMatch}{AuthorizationPrefix}/captcha";
        internal static string LoginPath => $"{PathMatch}{AuthorizationPrefix}/login.html";
        internal static string PathMatch { get; set; } = "/hangfire";
    }
}