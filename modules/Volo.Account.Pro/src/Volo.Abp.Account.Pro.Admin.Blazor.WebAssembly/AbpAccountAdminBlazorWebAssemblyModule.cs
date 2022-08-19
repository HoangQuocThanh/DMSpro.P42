using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement.Blazor.WebAssembly;

namespace Volo.Abp.Account.Pro.Admin.Blazor.WebAssembly;

[DependsOn(typeof(AbpAccountAdminBlazorModule),
    typeof(AbpSettingManagementBlazorWebAssemblyModule),
    typeof(AbpAccountAdminHttpApiClientModule))]
public class AbpAccountAdminBlazorWebAssemblyModule : AbpModule
{
}
