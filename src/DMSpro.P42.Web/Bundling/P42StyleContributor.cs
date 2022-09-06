using System.Collections.Generic;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace DMSpro.P42.Web.Bundling;
public class P42StyleContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        context.Files.AddIfNotContains("/libs/devextreme/css/dx.common.css"); 
        context.Files.AddIfNotContains("/libs/devextreme/css/dx.light.css");
    }
}
