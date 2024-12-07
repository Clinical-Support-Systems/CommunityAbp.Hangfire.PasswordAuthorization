using Localization.Resources.AbpUi;
using CommunityAbp.Hangfire.PasswordAuthorization.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace CommunityAbp.Hangfire.PasswordAuthorization;

[DependsOn(
    typeof(PasswordAuthorizationApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class PasswordAuthorizationHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(PasswordAuthorizationHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<PasswordAuthorizationResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
