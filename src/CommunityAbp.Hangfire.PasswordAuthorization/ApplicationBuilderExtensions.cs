using Hangfire;
using Hangfire.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace CommunityAbp.Hangfire.PasswordAuthorization;

/// <summary>
/// Extension methods for configuring Hangfire dashboard with password authorization.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds Hangfire dashboard with password authorization.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configureAuth">A delegate to configure the Hangfire authorization options.</param>
    /// <param name="configureDashboard">A delegate to configure the Hangfire dashboard options.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="configureAuth"/> is missing a username or password.</exception>
    public static IServiceCollection AddAbpHangfireDashboardWithLogin(this IServiceCollection services,
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
            Authorization = [new HangfirePasswordAuthorizationFilterAttribute()]
        };
        configureDashboard?.Invoke(dashboardOptions);

        // Register dashboard options as a singleton since they're not using IOptions
        services.AddSingleton(dashboardOptions);
        services.AddMemoryCache();

        return services;
    }

    /// <summary>
    /// Adds Hangfire dashboard with password authorization.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
    /// <param name="pathMatch">The path to match for the Hangfire dashboard.</param>
    /// <param name="storage">The storage to use for Hangfire.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> so that additional calls can be chained.</returns>
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