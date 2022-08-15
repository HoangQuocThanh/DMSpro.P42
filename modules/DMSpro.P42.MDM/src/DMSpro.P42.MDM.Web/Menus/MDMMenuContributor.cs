using System.Threading.Tasks;
using Volo.Abp.UI.Navigation;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Features;
using DMSpro.P42.MDM.Features;
using DMSpro.P42.MDM.Localization;

namespace DMSpro.P42.MDM.Web.Menus;

public class MDMMenuContributor : IMenuContributor
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
        var l = context.GetLocalizer<MDMResource>();

        var moduleMenu = new ApplicationMenuItem(
            MDMMenus.Prefix,
            displayName: l["Menu:MDM"],
            "~/MDM",
            icon: "fa fa-table")
            .RequireFeatures(MDMFeature.EnableMDM);

        //Add main menu items.
        context.Menu.Items.AddIfNotContains(moduleMenu);
        return moduleMenu;
    }
}