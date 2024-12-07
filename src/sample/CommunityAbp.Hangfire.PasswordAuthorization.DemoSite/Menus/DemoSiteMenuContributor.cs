using CommunityAbp.Hangfire.PasswordAuthorization.DemoSite.Permissions;
using CommunityAbp.Hangfire.PasswordAuthorization.DemoSite.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.UI.Navigation;
using Volo.Abp.AuditLogging.Web.Navigation;
using Volo.Abp.LanguageManagement.Navigation;
using Volo.Abp.OpenIddict.Pro.Web.Menus;
using Volo.Abp.TextTemplateManagement.Web.Navigation;
using Volo.Saas.Host.Navigation;

namespace CommunityAbp.Hangfire.PasswordAuthorization.DemoSite.Menus;

public class DemoSiteMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private static Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<DemoSiteResource>();
        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                DemoSiteMenus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fas fa-home",
                order: 0
            )
        );


        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 5;
        
        administration.SetSubItemOrder(SaasHostMenuNames.GroupName, 1);
        
        //Administration->OpenIddict
        administration.SetSubItemOrder(OpenIddictProMenus.GroupName, 3);

        //Administration->Language Management
        administration.SetSubItemOrder(LanguageManagementMenuNames.GroupName,4);
        
        //Administration->Text Template Management
        administration.SetSubItemOrder(TextTemplateManagementMainMenuNames.GroupName, 5);

        //Administration->Audit Logs
        administration.SetSubItemOrder(AbpAuditLoggingMainMenuNames.GroupName, 6);
        //Administration->Identity
        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);

        //Administration->Settings
        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 7);
        
        return Task.CompletedTask;
    }
}
