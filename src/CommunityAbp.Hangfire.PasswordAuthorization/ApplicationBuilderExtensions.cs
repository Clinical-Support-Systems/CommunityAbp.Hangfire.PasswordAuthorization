using Hangfire;
using Hangfire.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CommunityAbp.Hangfire.PasswordAuthorization
{
    public static class ApplicationBuilderExtensions
    {
        public static IServiceCollection AddAbpHangfireDashboardWithLogin(
            this IServiceCollection services,
            Action<HangfireAuthorizationOptions> configureAuth,
            Action<DashboardOptions>? configureDashboard = null)
        {
            // Validate parameters
            ArgumentNullException.ThrowIfNull(configureAuth);

            // Use Configure<HangfireAuthorizationOptions> to integrate with IOptions pattern
            services.Configure<HangfireAuthorizationOptions>(options =>
            {
                configureAuth(options);

                // Validate authorization options
                if (string.IsNullOrWhiteSpace(options.Username))
                    throw new ArgumentException("Username must not be empty", nameof(configureAuth));
                if (string.IsNullOrWhiteSpace(options.Password))
                    throw new ArgumentException("Password must not be empty", nameof(configureAuth));
            });

            // Configure dashboard options
            var dashboardOptions = new DashboardOptions
            {
                Authorization = new[] { new HangfirePasswordAuthorizationFilter() }
            };
            configureDashboard?.Invoke(dashboardOptions);

            // Register dashboard options as a singleton since they're not using IOptions
            services.AddSingleton(dashboardOptions);
            services.AddMemoryCache();

            return services;
        }

        public static IApplicationBuilder UseAbpHangfireDashboardWithLogin(
            [NotNull] this IApplicationBuilder app,
            [NotNull] string pathMatch = "/hangfire",
            [CanBeNull] JobStorage? storage = null)
        {
            ArgumentNullException.ThrowIfNull(app);
            ArgumentNullException.ThrowIfNull(pathMatch);

            // Normalize path
            pathMatch = pathMatch.StartsWith("/") ? pathMatch : $"/{pathMatch}";
            DashboardAuthorizationRouteConfig.PathMatch = pathMatch;

            var dashboardOptions = app.ApplicationServices.GetRequiredService<DashboardOptions>();

            // Map the authentication middleware and dashboard to the same path
            app.Map(new PathString(pathMatch), builder =>
            {
                builder.UseMiddleware<HangfireAuthenticationMiddleware>();
                builder.UseAbpHangfireDashboard("", options =>
                {
                    // Copy authorization settings from our configured options
                    options.Authorization = dashboardOptions.Authorization;

                    // Copy other dashboard settings
                    options.AppPath = dashboardOptions.AppPath;
                    options.DashboardTitle = dashboardOptions.DashboardTitle;
                    options.StatsPollingInterval = dashboardOptions.StatsPollingInterval;
                    options.DisplayStorageConnectionString = dashboardOptions.DisplayStorageConnectionString;
                    // Add any other options you want to copy over
                }, storage);
            });

            return app;
        }
    }
}