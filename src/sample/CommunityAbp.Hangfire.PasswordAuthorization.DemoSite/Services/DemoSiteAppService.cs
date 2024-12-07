using Volo.Abp.Application.Services;
using CommunityAbp.Hangfire.PasswordAuthorization.DemoSite.Localization;

namespace CommunityAbp.Hangfire.PasswordAuthorization.DemoSite.Services;

/* Inherit your application services from this class. */
public abstract class DemoSiteAppService : ApplicationService
{
    protected DemoSiteAppService()
    {
        LocalizationResource = typeof(DemoSiteResource);
    }
}