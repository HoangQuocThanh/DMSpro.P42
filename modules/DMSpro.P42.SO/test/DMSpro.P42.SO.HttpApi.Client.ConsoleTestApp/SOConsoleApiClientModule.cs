using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace DMSpro.P42.SO;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(SOHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class SOConsoleApiClientModule : AbpModule
{

}
