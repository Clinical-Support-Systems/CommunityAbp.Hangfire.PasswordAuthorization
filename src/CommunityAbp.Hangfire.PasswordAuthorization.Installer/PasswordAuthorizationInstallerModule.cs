using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace CommunityAbp.Hangfire.PasswordAuthorization;

[DependsOn(
    typeof(AbpVirtualFileSystemModule)
    )]
public class PasswordAuthorizationInstallerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<PasswordAuthorizationInstallerModule>();
        });
    }
}
