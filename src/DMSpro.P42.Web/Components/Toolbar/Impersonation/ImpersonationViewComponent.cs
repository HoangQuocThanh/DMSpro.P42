using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace DMSpro.P42.Web.Components.Toolbar.Impersonation;

public class ImpersonationViewComponent : AbpViewComponent
{
    public virtual IViewComponentResult Invoke()
    {
        return View("~/Components/Toolbar/Impersonation/Default.cshtml");
    }
}
