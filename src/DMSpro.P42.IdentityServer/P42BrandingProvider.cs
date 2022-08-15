using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace DMSpro.P42;

[Dependency(ReplaceServices = true)]
public class P42BrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "P42";
}
