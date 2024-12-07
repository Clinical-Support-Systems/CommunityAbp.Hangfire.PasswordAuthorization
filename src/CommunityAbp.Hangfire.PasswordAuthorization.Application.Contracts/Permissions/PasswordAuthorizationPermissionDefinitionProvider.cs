using CommunityAbp.Hangfire.PasswordAuthorization.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace CommunityAbp.Hangfire.PasswordAuthorization.Permissions;

public class PasswordAuthorizationPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(PasswordAuthorizationPermissions.GroupName, L("Permission:PasswordAuthorization"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<PasswordAuthorizationResource>(name);
    }
}
