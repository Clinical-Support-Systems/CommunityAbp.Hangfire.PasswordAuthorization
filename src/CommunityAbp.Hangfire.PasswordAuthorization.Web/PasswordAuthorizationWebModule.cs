using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using CommunityAbp.Hangfire.PasswordAuthorization.Localization;
using CommunityAbp.Hangfire.PasswordAuthorization.Web.Menus;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;
using CommunityAbp.Hangfire.PasswordAuthorization.Permissions;

namespace CommunityAbp.Hangfire.PasswordAuthorization.Web;

[DependsOn(
    typeof(PasswordAuthorizationApplicationContractsModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpAutoMapperModule)
    )]
public class PasswordAuthorizationWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(typeof(PasswordAuthorizationResource), typeof(PasswordAuthorizationWebModule).Assembly);
        });

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(PasswordAuthorizationWebModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new PasswordAuthorizationMenuContributor());
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<PasswordAuthorizationWebModule>();
        });

        context.Services.AddAutoMapperObjectMapper<PasswordAuthorizationWebModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<PasswordAuthorizationWebModule>(validate: true);
        });

        Configure<RazorPagesOptions>(options =>
        {
                //Configure authorization.
            });
    }
}
