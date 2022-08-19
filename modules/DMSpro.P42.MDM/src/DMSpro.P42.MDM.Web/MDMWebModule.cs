using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using DMSpro.P42.MDM.Localization;
using DMSpro.P42.MDM.Web.Menus;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;
using DMSpro.P42.MDM.Permissions;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.PageToolbars;
using Volo.Abp.Localization;
using DMSpro.P42.MDM.Web.Pages.MDM.Companies.ImportToolbar;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;

namespace DMSpro.P42.MDM.Web;

[DependsOn(
    typeof(MDMApplicationContractsModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpAutoMapperModule)
    )]
public class MDMWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(typeof(MDMResource), typeof(MDMWebModule).Assembly);
        });

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(MDMWebModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new MDMMenuContributor());
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<MDMWebModule>();
        });

        context.Services.AddAutoMapperObjectMapper<MDMWebModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<MDMWebModule>(validate: true);
        });

        Configure<RazorPagesOptions>(options =>
        {
            //Configure authorization.
            options.Conventions.AuthorizePage("/Companies/Index", MDMPermissions.Companies.Default);
        });

        Configure<AbpPageToolbarOptions>(options =>
        {
            options.Configure<Pages.MDM.Companies.IndexModel>(
                toolbar =>
                {
                    toolbar.AddComponent<ImportDropdownViewComponent>(requiredPolicyName: MDMPermissions.Companies.Import);

                    toolbar.AddButton(
                        LocalizableString.Create<MDMResource>("ImportExcel"),
                        icon: "excel",
                        name: "ImportExcel",
                        requiredPolicyName: MDMPermissions.Companies.Create
                    );
                }
            );
        });

        Configure<AbpToolbarOptions>(options =>
        {
            options.Contributors.Add(new MDMToolbarContributor());
        });
    }
}