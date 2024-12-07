using CommunityAbp.Hangfire.PasswordAuthorization.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace CommunityAbp.Hangfire.PasswordAuthorization.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class PasswordAuthorizationPageModel : AbpPageModel
{
    protected PasswordAuthorizationPageModel()
    {
        LocalizationResourceType = typeof(PasswordAuthorizationResource);
        ObjectMapperContext = typeof(PasswordAuthorizationWebModule);
    }
}
