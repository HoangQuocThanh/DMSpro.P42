using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace DMSpro.P42.Web;

[Dependency(ReplaceServices = true)]
public class P42BrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "P42";
}
