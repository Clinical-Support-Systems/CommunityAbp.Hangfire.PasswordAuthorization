using Microsoft.Extensions.Localization;
using CommunityAbp.Hangfire.PasswordAuthorization.DemoSite.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace CommunityAbp.Hangfire.PasswordAuthorization.DemoSite;

[Dependency(ReplaceServices = true)]
public class DemoSiteBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<DemoSiteResource> _localizer;

    public DemoSiteBrandingProvider(IStringLocalizer<DemoSiteResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}