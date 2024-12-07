using Volo.Abp.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace CommunityAbp.Hangfire.PasswordAuthorization.DemoSite.Data;

public class DemoSiteDbSchemaMigrator : ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public DemoSiteDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        
        /* We intentionally resolving the DemoSiteDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<DemoSiteDbContext>()
            .Database
            .MigrateAsync();

    }
}
