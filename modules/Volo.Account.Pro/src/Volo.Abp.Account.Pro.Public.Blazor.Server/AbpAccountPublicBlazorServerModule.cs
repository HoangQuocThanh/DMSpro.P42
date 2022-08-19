using Volo.Abp.AspNetCore.Components.Server;
using Volo.Abp.AspNetCore.Components.Web.Theming;
using Volo.Abp.AspNetCore.Components.Web.Theming.Toolbars;
using Volo.Abp.Modularity;

namespace Volo.Abp.Account.Pro.Public.Blazor.Server;

[DependsOn(
    typeof(AbpAspNetCoreComponentsServerModule),
    typeof(AbpAspNetCoreComponentsWebThemingModule),
    typeof(AbpAccountSharedApplicationContractsModule)
)]
public class AbpAccountPublicBlazorServerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpToolbarOptions>(options =>
        {
            options.Contributors.Add(new AccountBlazorToolbarContributor());
        });
    }
}
