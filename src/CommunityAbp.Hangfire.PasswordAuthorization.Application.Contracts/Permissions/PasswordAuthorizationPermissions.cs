using Volo.Abp.Reflection;

namespace CommunityAbp.Hangfire.PasswordAuthorization.Permissions;

public class PasswordAuthorizationPermissions
{
    public const string GroupName = "PasswordAuthorization";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(PasswordAuthorizationPermissions));
    }
}
