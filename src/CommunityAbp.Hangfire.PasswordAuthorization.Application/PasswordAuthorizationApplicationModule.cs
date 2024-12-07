using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.Application;

namespace CommunityAbp.Hangfire.PasswordAuthorization;

[DependsOn(
    typeof(PasswordAuthorizationDomainModule),
    typeof(PasswordAuthorizationApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule)
    )]
public class PasswordAuthorizationApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<PasswordAuthorizationApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<PasswordAuthorizationApplicationModule>(validate: true);
        });
    }
}
