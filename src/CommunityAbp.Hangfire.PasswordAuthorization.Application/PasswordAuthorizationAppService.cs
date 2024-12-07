using CommunityAbp.Hangfire.PasswordAuthorization.Localization;
using Volo.Abp.Application.Services;

namespace CommunityAbp.Hangfire.PasswordAuthorization;

public abstract class PasswordAuthorizationAppService : ApplicationService
{
    protected PasswordAuthorizationAppService()
    {
        LocalizationResource = typeof(PasswordAuthorizationResource);
        ObjectMapperContext = typeof(PasswordAuthorizationApplicationModule);
    }
}
