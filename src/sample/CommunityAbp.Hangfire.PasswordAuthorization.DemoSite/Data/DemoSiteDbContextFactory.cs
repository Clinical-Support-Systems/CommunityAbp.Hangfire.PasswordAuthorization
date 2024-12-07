using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CommunityAbp.Hangfire.PasswordAuthorization.DemoSite.Data;

public class DemoSiteDbContextFactory : IDesignTimeDbContextFactory<DemoSiteDbContext>
{
    public DemoSiteDbContext CreateDbContext(string[] args)
    {
        DemoSiteEfCoreEntityExtensionMappings.Configure();
        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<DemoSiteDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));

        return new DemoSiteDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}