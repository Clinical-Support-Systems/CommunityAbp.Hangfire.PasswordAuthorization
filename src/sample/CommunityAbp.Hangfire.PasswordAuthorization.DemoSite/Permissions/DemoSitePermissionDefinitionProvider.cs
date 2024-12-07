using CommunityAbp.Hangfire.PasswordAuthorization.DemoSite.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace CommunityAbp.Hangfire.PasswordAuthorization.DemoSite.Permissions;

public class DemoSitePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(DemoSitePermissions.GroupName);

        
        //Define your own permissions here. Example:
        //myGroup.AddPermission(DemoSitePermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<DemoSiteResource>(name);
    }
}
