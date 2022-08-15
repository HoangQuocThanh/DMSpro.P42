using Volo.Abp.Modularity;

namespace DMSpro.P42.MDM;

[DependsOn(
    typeof(MDMApplicationModule),
    typeof(MDMDomainTestModule)
    )]
public class MDMApplicationTestModule : AbpModule
{

}
