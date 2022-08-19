using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement.Blazor.Server;

namespace Volo.Abp.Account.Pro.Admin.Blazor.Server;

[DependsOn(typeof(AbpAccountAdminBlazorModule),
    typeof(AbpSettingManagementBlazorServerModule))]
public class AbpAccountAdminBlazorServerModule : AbpModule
{
}
