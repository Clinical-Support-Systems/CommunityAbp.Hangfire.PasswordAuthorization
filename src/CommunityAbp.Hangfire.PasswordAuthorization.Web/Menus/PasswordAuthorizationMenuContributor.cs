using System.Threading.Tasks;
using Volo.Abp.UI.Navigation;

namespace CommunityAbp.Hangfire.PasswordAuthorization.Web.Menus;

public class PasswordAuthorizationMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        //Add main menu items.
        context.Menu.AddItem(new ApplicationMenuItem(PasswordAuthorizationMenus.Prefix, displayName: "PasswordAuthorization", "~/PasswordAuthorization", icon: "fa fa-globe"));

        return Task.CompletedTask;
    }
}
