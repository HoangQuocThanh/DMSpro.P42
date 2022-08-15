using System.Threading.Tasks;
using Volo.Abp.UI.Navigation;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Features;
using DMSpro.P42.SO.Features;
using DMSpro.P42.SO.Localization;

namespace DMSpro.P42.SO.Web.Menus;

public class SOMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name != StandardMenus.Main)
        {
            return;
        }

        var moduleMenu = AddModuleMenuItem(context); //Do not delete `moduleMenu` variable as it will be used by ABP Suite!
    }

    private static ApplicationMenuItem AddModuleMenuItem(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<SOResource>();

        var moduleMenu = new ApplicationMenuItem(
            SOMenus.Prefix,
            displayName: l["Menu:SO"],
            "~/SO",
            icon: "fa fa-pencil-square-o")
            .RequireFeatures(SOFeature.EnableSO);

        //Add main menu items.
        context.Menu.Items.AddIfNotContains(moduleMenu);
        return moduleMenu;
    }
}