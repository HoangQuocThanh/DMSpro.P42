using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace DMSpro.P42.MDM;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(MDMHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class MDMConsoleApiClientModule : AbpModule
{

}
