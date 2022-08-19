using Microsoft.AspNetCore.Authorization;
using DMSpro.P42.MDM.Permissions;
using System.Threading.Tasks;
using Volo.Abp.UI.Navigation;
using System.Collections.Generic;
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

        AddMenuItemCompanies(context, moduleMenu);
        //AddMenuItemUsers(context, moduleMenu);
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

    private static void AddMenuItemCompanies(MenuConfigurationContext context, ApplicationMenuItem parentMenu)
    {
        parentMenu.AddItem(
            new ApplicationMenuItem(
                Menus.MDMMenus.Companies,
                context.GetLocalizer<MDMResource>()["Menu:Companies"],
                "/MDM/Companies",
                icon: "fa fa-file-alt",
                requiredPermissionName: MDMPermissions.Companies.Default
            )
        );
    }
    //private static void AddMenuItemUsers(MenuConfigurationContext context, ApplicationMenuItem parentMenu)
    //{
    //    parentMenu.AddItem(
    //        new ApplicationMenuItem(
    //            Menus.MDMMenus.Users,
    //            context.GetLocalizer<MDMResource>()["Menu:Users"],
    //            "~/Identity/Users",
    //            icon: "fa fa-user",
    //            requiredPermissionName: MDMPermissions.Users.Default
    //        )
    //    );
    //}
}