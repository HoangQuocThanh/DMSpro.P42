using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Volo.Abp.Identity.Web.Pages.Identity.Users.ImportToolbar;

public class ImportDropdownViewComponent : AbpViewComponent
{
    public virtual IViewComponentResult Invoke()
    {
        return View("~/Pages/Identity/Users/ImportToolbar/Default.cshtml");
    }
}