using CommunityAbp.Hangfire.PasswordAuthorization.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace CommunityAbp.Hangfire.PasswordAuthorization;

public abstract class PasswordAuthorizationController : AbpControllerBase
{
    protected PasswordAuthorizationController()
    {
        LocalizationResource = typeof(PasswordAuthorizationResource);
    }
}
