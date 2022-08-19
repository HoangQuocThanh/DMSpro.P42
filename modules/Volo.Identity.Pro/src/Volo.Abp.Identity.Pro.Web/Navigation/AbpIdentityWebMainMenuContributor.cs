using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Volo.Abp.Identity.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.UI.Navigation;

namespace Volo.Abp.Identity.Web.Navigation;

public class AbpIdentityWebMainMenuContributor : IMenuContributor
{
    public virtual Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name != StandardMenus.Main)
        {
            return Task.CompletedTask;
        }

        var l = context.GetLocalizer<IdentityResource>();

        var identityMenuItem = new ApplicationMenuItem(IdentityMenuNames.GroupName, l["Menu:IdentityManagement"], icon: "fa fa-id-card-o");

        identityMenuItem.AddItem(new ApplicationMenuItem(IdentityMenuNames.OrganizationUnits, l["OrganizationUnits"], url: "~/Identity/OrganizationUnits").RequirePermissions(IdentityPermissions.OrganizationUnits.Default));
        identityMenuItem.AddItem(new ApplicationMenuItem(IdentityMenuNames.Roles, l["Roles"], url: "~/Identity/Roles").RequirePermissions(IdentityPermissions.Roles.Default));
        identityMenuItem.AddItem(new ApplicationMenuItem(IdentityMenuNames.Users, l["Users"], url: "~/Identity/Users").RequirePermissions(IdentityPermissions.Users.Default));
        identityMenuItem.AddItem(new ApplicationMenuItem(IdentityMenuNames.ClaimTypes, l["ClaimTypes"], url: "~/Identity/ClaimTypes").RequirePermissions(IdentityPermissions.ClaimTypes.Default));
        identityMenuItem.AddItem(new ApplicationMenuItem(IdentityMenuNames.SecurityLogs, l["SecurityLogs"], url: "~/Identity/SecurityLogs").RequirePermissions(IdentityPermissions.SecurityLogs.Default));

        context.Menu.GetAdministration().AddItem(identityMenuItem);

        return Task.CompletedTask;
    }
}
