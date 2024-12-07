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
