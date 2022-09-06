using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace DMSpro.P42.eRoute;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(eRouteHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class eRouteConsoleApiClientModule : AbpModule
{

}
