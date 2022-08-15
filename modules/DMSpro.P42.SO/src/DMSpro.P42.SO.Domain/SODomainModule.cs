using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace DMSpro.P42.SO;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(SODomainSharedModule)
)]
public class SODomainModule : AbpModule
{

}
