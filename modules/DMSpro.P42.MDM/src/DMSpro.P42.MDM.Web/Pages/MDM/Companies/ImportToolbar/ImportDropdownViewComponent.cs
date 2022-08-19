using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace DMSpro.P42.MDM.Web.Pages.MDM.Companies.ImportToolbar;

public class ImportDropdownViewComponent : AbpViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View("~/Pages/MDM/Companies/ImportToolbar/Default.cshtml");
    }
}