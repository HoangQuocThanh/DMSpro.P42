using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace DMSpro.P42.MDM;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(MDMDomainSharedModule)
)]
public class MDMDomainModule : AbpModule
{

}
