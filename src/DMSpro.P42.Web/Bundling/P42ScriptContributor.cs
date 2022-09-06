using System.Collections.Generic;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Packages.JQuery;
using Volo.Abp.Modularity;

namespace DMSpro.P42.Web.Bundling;

[DependsOn(typeof(JQueryScriptContributor))]
public class P42ScriptContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        context.Files.AddIfNotContains("/libs/devextreme/js/dx.all.js");
        context.Files.AddIfNotContains("/libs/devextreme/js/dx.aspnet.mvc.js");
        context.Files.AddIfNotContains("/libs/devextreme/js/dx.aspnet.data.js");
    }
}
