using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace CommunityAbp.Hangfire.PasswordAuthorization;

[DependsOn(
    typeof(PasswordAuthorizationApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class PasswordAuthorizationHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(PasswordAuthorizationApplicationContractsModule).Assembly,
            PasswordAuthorizationRemoteServiceConsts.RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<PasswordAuthorizationHttpApiClientModule>();
        });

    }
}
